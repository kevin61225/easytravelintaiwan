/**
 * Creates a DirectionsRoute object.
 *
 * @constructor
 * @param {DirectionsDisplay} directionsDisplay The display object.
 */
function DirectionsRoute(directionsDisplay) {
    this.directionsDisplay = directionsDisplay;

	this.directionsService = new google.maps.DirectionsService();
}

/**
 * Calculate path between destinations.
 *
 * @param {Array<google.maps.DirectionsWaypoint>} destinations The list of destinations
 *				to visit.
 * @param {google.maps.DirectionsTravelMode} selectedMode The type of traveling: car, bike, or walking
 * @param {bool} hwy Whether to avoid highways
 * @param {bool} toll Whether to avoid tolled roads
 * @param {bool} onlyCurrent If using multiple routes, do we want to show all of them or just the current
 * @param {string} units The distance units to use, either "km" or "mi"
 */
DirectionsRoute.prototype.route = function(destinations, selectedMode, hwy, toll, onlyCurrent, units) {
	this.directionsDisplay.reset();
	
	// Determine unit system.
	var unitSystem = google.maps.DirectionsUnitSystem.IMPERIAL;
	if(units == "km")
		unitSystem = google.maps.DirectionsUnitSystem.METRIC;

	// Loop through all destinations in groups of 10, and find route to display.
	for(var idx1=0; idx1<destinations.length-1; idx1+=9)
	{
		// Setup options.
		var idx2 = Math.min(idx1+9, destinations.length-1);
        var request = {
			avoidHighways: hwy,
			avoidTolls: toll,
			origin: destinations[idx1].location,
			destination: destinations[idx2].location,
			travelMode: google.maps.DirectionsTravelMode[selectedMode],
			unitSystem: unitSystem,
			waypoints: destinations.slice(idx1+1, idx2)
		};
		
		// Determine path and display results.
		this.directionsService.route(request, function (response, status) {
			if (status == google.maps.DirectionsStatus.OK)
				this.directionsDisplay.parse(response, units);
		});
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////


/**
 * Creates a DirectionsDisplay object.
 *
 * @constructor
 * @param {google.maps.Map} map The map to display the directions.
 * @param {DivObject} pane The div pane to put the step-by-step directions.
 */
markers = new Array();
function DirectionsDisplay(map, pane) {
	this.geocoder = new google.maps.Geocoder();
	this.legs = new Array();
	this.distances = new Array();
	this.overallDistance = 0;
	this.map = map;
	this.pane = pane;
	this.suppressMarkers = true;
	this.draggable = true;
	var rendererOptions = {
	    suppressMarkers: true,
	    draggable: true
	};
}

/**
 * Generates boxes for a given route and distance
 *
 * @param {google.maps.DirectionsResult} response The result of calculating
 *				directions through the destinations.
 */
DirectionsDisplay.prototype.parse = function (response, units) {
	var routes = response.routes;
	
	// Loop through all routes and append
	for(var rte in routes)
	{
		var legs = routes[rte].legs;
		this.add_leg_(routes[rte].overview_path);
		
		for(var leg in legs)
		{
			var steps = legs[leg].steps;
			
			// Compute overall distance and time for the trip.
			this.overallDistance += legs[leg].distance.value;
			this.overallTime += legs[leg].duration.value;
		}
	}

	// Set zoom and center of map to fit all paths, and display directions.
	this.fit_route_();
	this.create_stepbystep_(response, units);
}

/**
 * Clears all stored information about the previous directions.
 */
DirectionsDisplay.prototype.reset = function () {
	// Delete all markers.
	//for(var x in this.markers) {
	//	this.markers[x].setMap(null);
	//}
	//this.markers = new Array();
	
	// Delete all polylines.
	for(var x in this.legs) {
		this.legs[x].setMap(null);
	}
	this.legs = new Array();
	
	// Delete all stored distances.
	for(var x in this.distances) {
		this.distances[x].setMap(null);
	}
	this.distances = new Array();
	
	// Reset overall counters.
	this.overallDistance = 0;
	this.overallTime = 0;
}

/**
 * Displays a marker on the map, whether it is an address or a lat/lon.
 *
 * @param {google.maps.LatLng | string} location The location to put the marker
 */
DirectionsDisplay.prototype.add_marker_ = function (location) {
	// Determine location
	if(isString(location)) {
		this.geocoder.geocode({address: location}, function (results, status) {
			if(status == google.maps.GeocoderStatus.OK) {
				var places = [results[0].formatted_address, results[0].geometry.location];
				// Add a marker, with incrementing characters on the icons.
				var letter = String.fromCharCode(65 + markers.length);
				markers.push(new google.maps.Marker({
					position:  places[1],
					map:  this.map,
					icon: "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=" + letter + "|FF0000|000000"
				}));
			}
		});
	}
	else {
		// Add a marker, with incrementing characters on the icons.
		var letter = String.fromCharCode(65 + markers.length);
		markers.push(new google.maps.Marker({
			position:  location,
			map:  this.map,
			icon: "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=" + letter + "|FF0000|000000"
		}));
	}
}

// Return a boolean value telling whether the first argument is a string. 
function isString(val) {
	if (typeof(val) == 'string') return true;
	if (typeof(val) == 'object') {
		var criterion = arguments[0].constructor.toString().match(/string/i); 
		return (criterion != null);
	}
	return false;
}


/**
 * Displays a PolyLine of points on the map.
 *
 * @param {Array<google.maps.LatLng>} path The path of the leg
 */
DirectionsDisplay.prototype.add_leg_ = function (path) {
	this.legs.push(new google.maps.Polyline({
		path: path,
		map: this.map,
		strokeColor: "#0000FF",
		strokeOpacity: 0.7,
		strokeWeight: 4}));
}

/**
 * Resize plot screen to fit route.
 */
DirectionsDisplay.prototype.fit_route_ = function () {
	// Go through all legs of route and fit plot.
	var latlngbounds = new google.maps.LatLngBounds();
	for(var leg in this.legs) {
		path = this.legs[leg].getPath();
		for(var i = 0; i < path.length; i++)
			latlngbounds.extend(path.getAt(i));
	}

	map.fitBounds(latlngbounds);
}

/**
 * Output step-by-step instructions to a div pane. Also setup mouse events.
 *
 * @param {maps.google.DirectionsResponse} response The response to Googles DirectionsService
 * @param {string} units The distance units to use, either "km" or "mi"
 */
DirectionsDisplay.prototype.create_stepbystep_ = function (response, units) {
	this.pane.innerHTML = "<br>Total Distance: " + this.compute_distance_(this.overallDistance, units);
	this.pane.innerHTML += "<br>Total Time: " + this.secs_to_hrmins_(this.overallTime) + "<br>";
		
	if(response.routes[0].warnings.length > 0) this.pane.innerHTML += "<br>";
	for(var i = 0; i < response.routes[0].warnings.length; i++)
		this.pane.innerHTML += "<b><i>Warning: </i></b>" + response.routes[0].warnings[i] + "<br>";;
	
	var htmlText = "<table id='tableDirections'>";

	var routes = response.routes;
	for(var rte in routes) {
		var legs = routes[rte].legs;
		for(var leg = 0; leg < legs.length; leg++) {
			var steps = legs[leg].steps;
			var letter1 = String.fromCharCode(65 + leg);
			var letter2 = String.fromCharCode(65 + leg+1);
			htmlText += "<br>";
			htmlText += "<tr><th colspan=2><hr></th></tr>";
			htmlText += "<tr><th colspan=2 align='center'><u>Directions from " + letter1 + " to " + letter2 + "</u></th></tr>";
			var totalDist = 0;
			var totalDur = 0;
			for(var x = 0; x < steps.length; x++) {
				htmlText += "<tr id = 'step" + x + "'>";
				htmlText += "<td valign='top'><b>" + (x+1) + " </b></td>";
				htmlText += "<td>" + steps[x].instructions + "</td>";
				htmlText += "</tr>";
				htmlText += "<tr id='time" + x + "'>";
				htmlText += "<td> &nbsp;</td>"
				htmlText += "<td align='left'><i>Duration: " + steps[x].distance.text + ", " + steps[x].duration.text + "</i></td>";
				htmlText += "</tr>";
				totalDist += steps[x].distance.value;
				totalDur += steps[x].duration.value;
			}
			htmlText += "<tr><th colspan=2 align='left'>\Leg Distance: " + this.compute_distance_(totalDist, units) + "</th></tr>";
			htmlText += "<tr><th colspan=2 align='left'>\Leg Duration: " + this.secs_to_hrmins_(totalDur) + "</th></tr>";
		}
	}
	this.pane.innerHTML += htmlText + "</table><br>" + response.routes[0].copyrights;
}

/**
 * Convert distance from meters to either kilometers or miles for output.
 *
 * @param {number} distance The distance in meters
 * @param {string} units The distance units to use, either "km" or "mi"
 *
 * @return {string} The string containing the distance converted to appropriate coordinates
 */
DirectionsDisplay.prototype.compute_distance_ = function (distance, units) {
	if(units == "km")
		return Math.round((this.overallDistance/1000)*100)/100 + " km"
	else
		return Math.round(this.overallDistance*0.000621371192*100)/100 + " mi";
}

/**
 * Convert time from seconds to an hours-minutes format.
 *
 * @param {number} time The time in seconds
 *
 * @return {string} The string containing the time converted to appropriate units
 */
DirectionsDisplay.prototype.secs_to_hrmins_ = function (time) {
	var hrs = Math.floor(time/3600);
	var mins = Math.round(time/60 - hrs*60);
	if(hrs > 0 && mins > 0)
		return hrs + " hours " + mins + " minutes";
	else if(mins > 0)
		return mins + " minutes";
	else if(hrs > 0)
		return hrs + " hours";
	else
		return "0 minutes";
}
