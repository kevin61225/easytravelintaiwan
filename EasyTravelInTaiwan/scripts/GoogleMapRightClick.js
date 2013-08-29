function InitRightClick() {

    //var gmap = map;
    var MaxInputs = 8; //maximum input boxes allowed
    var InputsWrapper = $("#InputsWrapper"); //Input boxes wrapper ID
    var AddButton = $("#AddMoreFileBox"); //Add button ID

    var x = InputsWrapper.length; //initlal text box count
    var FieldCount = 1; //to keep track of text box added

    var directionsRendererOptions = {};
    directionsRendererOptions.draggable = true;
    directionsRendererOptions.hideRouteList = true;
    directionsRendererOptions.suppressMarkers = true;
    directionsRendererOptions.preserveViewport = false;
    var directionsRenderer = new google.maps.DirectionsRenderer(directionsRendererOptions);
    var directionsService = new google.maps.DirectionsService();

    var tempWaypt = '';
    var markerIndex;
    var isCalculated = false;

    var contextMenuOptions = {};
    contextMenuOptions.classNames = { menu: 'context_menu', menuSeparator: 'context_menu_separator' };

    //	create an array of ContextMenuItem objects
    //	an 'id' is defined for each of the four directions related items
    var menuItems = [];
    menuItems.push({ className: 'context_menu_item', eventName: 'directions_origin_click', id: 'directionsOriginItem', label: '設定起點' });
    menuItems.push({ className: 'context_menu_item', eventName: 'directions_destination_click', id: 'directionsDestinationItem', label: '設定終點' });
    menuItems.push({ className: 'context_menu_item', eventName: 'directions_waypoint_click', id: 'directionsWaypointItem', label: '我想來這裡' });
    menuItems.push({ className: 'context_menu_item', eventName: 'clear_directions_click', id: 'clearDirectionsItem', label: '清除路線' });
    menuItems.push({ className: 'context_menu_item', eventName: 'get_directions_click', id: 'getDirectionsItem', label: '進行路線規劃' });
    //	a menuItem with no properties will be rendered as a separator
    menuItems.push({});
    menuItems.push({ className: 'context_menu_item', eventName: 'zoom_in_click', label: '放大' });
    menuItems.push({ className: 'context_menu_item', eventName: 'zoom_out_click', label: '縮小' });
    menuItems.push({});
    menuItems.push({ className: 'context_menu_item', eventName: 'center_map_click', label: '設定中心點' });
    contextMenuOptions.menuItems = menuItems;

    contextMenu = new ContextMenu(gmap, contextMenuOptions);
    
    //google.maps.event.addListener(distanceWidget.radiusWidget.circle, 'click', function () {
    //    console.log("circle clicked !!");
    //    console.log(distanceWidget.radiusWidget.circle);
    //    contextMenu.hide();
    //});

    google.maps.event.addListener(gmap, 'rightclick', function (marker) {
        //contextMenu.show(mouseEvent.latLng);
        try {
            contextMenu.show(marker.getPosition());
            tempWaypt = ''
            tempWaypt = marker.title;
            markerIndex = marker.gIndex;
            console.log(marker.gId);
        } catch (err) {

        }
    });

    //	create markers to show directions origin and destination
    //	both are not visible by default
    var markerOptions = {};
    markerOptions.icon = 'http://www.google.com/intl/en_ALL/mapfiles/markerA.png';
    markerOptions.map = null;
    markerOptions.position = new google.maps.LatLng(0, 0);
    markerOptions.title = '起點';
    markerOptions.visible = false;

    var originMarker = new google.maps.Marker(markerOptions);

    markerOptions.icon = 'http://www.google.com/intl/en_ALL/mapfiles/markerB.png';
    markerOptions.title = '終點';
    markerOptions.visible = false;
    var destinationMarker = new google.maps.Marker(markerOptions);

    //	listen for the ContextMenu 'menu_item_selected' event
    google.maps.event.addListener(contextMenu, 'menu_item_selected', function (latLng, eventName) {
        switch (eventName) {
            case 'directions_origin_click':
                originMarker.setPosition(latLng);
                if (!originMarker.getMap()) {
                    originMarker.setMap(gmap);
                }
                break;
            case 'directions_destination_click':
                destinationMarker.setPosition(latLng);
                if (!destinationMarker.getMap()) {
                    destinationMarker.setMap(gmap);
                }
                break;
            case 'clear_directions_click':
                originMarker.setMap(null);
                destinationMarker.setMap(null);
                directionsRenderer.setMap(null);
                waypts = [];
                isCalulated = false;
                $(InputsWrapper).empty();
                //	set CSS styles to defaults
                document.getElementById('clearDirectionsItem').style.display = '';
                document.getElementById('directionsDestinationItem').style.display = '';
                document.getElementById('directionsOriginItem').style.display = '';
                document.getElementById('getDirectionsItem').style.display = '';
                break;
            case 'directions_waypoint_click':
                if (!gmarkers[markerIndex].isChecked) {
                    waypts.push({
                        location: tempWaypt,
                        stopover: true
                    });
                    $("#InputsWrapper").append('<li id="place.' + gmarkers[markerIndex].gId + '"><a href="#">' + tempWaypt + '</a></li>');

                    gmarkers[markerIndex].isChecked = true;
                    x++; //text box increment
                    if (isCalculated) {
                        var directionsRequest = {};
                        directionsRequest.destination = destinationMarker.getPosition();
                        directionsRequest.origin = originMarker.getPosition();
                        directionsRequest.waypoints = waypts;
                        directionsRequest.travelMode = google.maps.TravelMode.DRIVING;

                        directionsService.route(directionsRequest, function (result, status) {
                            if (status === google.maps.DirectionsStatus.OK) {
                                //	hide the origin and destination markers as the DirectionsRenderer will render Markers itself
                                //originMarker.setMap(null);
                                //destinationMarker.setMap(null);
                                directionsRenderer.setDirections(result);
                                directionsRenderer.setMap(gmap);
                            } else {
                                alert('Sorry, the map was unable to obtain directions.\n\nThe request failed with the message: ' + status);
                            }
                        });
                        console.log("RE-Calculated");
                    }
                    console.log(waypts);
                }
                break;
            case 'get_directions_click':
                var directionsRequest = {};
                directionsRequest.destination = destinationMarker.getPosition();
                directionsRequest.origin = originMarker.getPosition();
                directionsRequest.waypoints = waypts;
                directionsRequest.travelMode = google.maps.TravelMode.DRIVING;

                directionsService.route(directionsRequest, function (result, status) {
                    if (status === google.maps.DirectionsStatus.OK) {
                        //	hide the origin and destination markers as the DirectionsRenderer will render Markers itself
                        //originMarker.setMap(null);
                        //destinationMarker.setMap(null);
                        directionsRenderer.setDirections(result);
                        directionsRenderer.setMap(gmap);
                        //	hide all but the 'Clear directions' menu item
                        document.getElementById('clearDirectionsItem').style.display = 'block';
                        document.getElementById('directionsDestinationItem').style.display = 'none';
                        document.getElementById('directionsOriginItem').style.display = 'none';
                        document.getElementById('getDirectionsItem').style.display = 'none';
                    } else {
                        alert('Sorry, the map was unable to obtain directions.\n\nThe request failed with the message: ' + status);
                    }
                });
                console.log("Calculated");
                isCalculated = true;
                break;
            case 'zoom_in_click':
                gmap.setZoom(gmap.getZoom() + 1);
                break;
            case 'zoom_out_click':
                gmap.setZoom(gmap.getZoom() - 1);
                break;
            case 'center_map_click':
                gmap.panTo(latLng);
                break;
            default:
                break;
        }
        if (originMarker.getMap() && destinationMarker.getMap() && document.getElementById('getDirectionsItem').style.display === '') {
            //	display the 'Get directions' menu item if it is not visible and both directions origin and destination have been selected
            document.getElementById('getDirectionsItem').style.display = 'block';
        }
    });
}