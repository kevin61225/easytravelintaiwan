var icons = new GoogleIcons();

$("#inputGeo").geocomplete({
    map: ".map_canvas",
    location: new google.maps.LatLng(25.039941, 121.512812),
    mapOptions: {
        zoom: 15,
        center: new google.maps.LatLng(25.039941, 121.512812),
        scrollwheel: true,
        scaleControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    },
    markerOptions: {
        draggable: true,
        icon: icons.mainIcon,
        visible: true,
        title: '移動我 !!'
    },
    details: "#status",
})
    .bind("geocode:result", function (event, result) {
        console.log("Result: " + result.formatted_address);
    })
    .bind("geocode:error", function (event, status) {
        console.log("ERROR: " + status);
    })
    .bind("geocode:multiple", function (event, results) {
        console.log("Multiple: " + results.length + " results found");
    })
    .bind("geocode:dragged", function (event, latLng) {
        //$("input[name=lat]").val(latLng.lat());
        //$("input[name=lng]").val(latLng.lng());
    });

var gmap = $("#inputGeo").geocomplete("map");
var gmarker = $("#inputGeo").geocomplete("marker");
var infowindow = new google.maps.InfoWindow();
var gmarkers = [];
var waypts = new Array();
var contextMenu;
var unloading = false;

var $progressBar = $('<div class="progress">' + 
                    '<div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 30%">' +
                    '<span class="sr-only">30% Complete</span>' +
                    '</div>' +
                    '</div>');

function loadPartialView($object, url) {
    $.ajax({
        url: url,
        type: "get",
        async: true,
        success: function (result) {
            $object.html(result);
        },
        error: function (message) {
            console.log(message)
        }
    });
    //$.get(url, function (data) {
    //    $object.html(data);
    //});
}

function initialize(mapData) {

    var imageBounds = new google.maps.LatLngBounds(new google.maps.LatLng(22.046803, 120.697474), new google.maps.LatLng(22.05, 120.797474));
    var historicalOverlay;
    historicalOverlay = new google.maps.GroundOverlay('/Content/images/PleaseZoomIn.png', imageBounds);
    historicalOverlay.setMap(gmap);

    $.each(mapData, function (i, item) {
        setupLocationMarker(item);
    });

    InitRightClick();

    //var distanceWidget = new DistanceWidget();

    //google.maps.event.addListener(gmarker, 'position_changed', function () {
    //    distanceWidget.set('position', gmarker.getPosition());
    //    displayInfo(distanceWidget);
    //});

    google.maps.event.addListener(gmap, "zoom_changed", function () {
        var zoom = this.getZoom();
        if (zoom < 17) {
            //hide("06");
            //hide("07");
            //hide("10");
            console.log("Please zoom in");
        }
        else {
            //$(".filterClass .btn").hasClass("active") {
            //    boxclick($(this).hasClass("active"), $(this).val());
            //}
        }
    });

    //google.maps.event.addListener(distanceWidget, 'distance_changed', function () {
    //    displayInfo(distanceWidget);
    //});

    //google.maps.event.addListener(distanceWidget, 'position_changed', function () {
    //    displayInfo(distanceWidget);
    //});

    console.log(gmarkers.length);
}

function setupLocationMarker(item) {

    var markerIcon;
    switch (item.Pt) {
        case "06": //住宿
            switch (item.IconType) {
                case "11":
                    markerIcon = icons.hotelIcon11;
                    break;
                case "12":
                    markerIcon = icons.hotelIcon12;
                    break;
                case "13":
                    markerIcon = icons.hotelIcon13;
                    break;
                case "14":
                    markerIcon = icons.hotelIcon14;
                    break;
            }
            //markerIcon = hotelIcon;
            break;
        case "07":
            markerIcon = icons.foodIcon;
            break;
        case "10":
            markerIcon = icons.viewIcon;
            break;
        case "20":
            markerIcon = icons.gasIcon;
            break;
        default:
            break;
    }

    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(item.Lat, item.Lng),
        map: gmap,
        title: item.Name,
        icon: markerIcon,
        visible: false,
        isChecked: false,
        gIndex: gmarkers.length,
        gId: item.Id
    });
    marker.category = item.Pt;

    gmarkers.push(marker);

    // add a marker click event
    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent('<p>' + item.Name + '</p><p>地址：' + item.Address + '</p><p>類型：' + item.Viewtype + '</p><a href="/Map/ViewPointDetails/' + item.Id + ' " target="_blank">點我看詳細內容</a>');
        infowindow.open(gmap, this);
        waypts.push({ gId: marker.gId, title: marker.title });
        $.cookie('wayptsCookie', waypts);
        $('#History').click();
    });

    google.maps.event.addListener(marker, 'rightclick', function () {
        google.maps.event.trigger(gmap, 'rightclick', this);
    });
}

// == shows all markers of a particular category, and ensures the checkbox is checked ==
function show(category) {
    for (var i = 0; i < gmarkers.length; i++) {
        if (gmarkers[i].category == category) {
            gmarkers[i].setVisible(true);
        }
    }
    // == check the checkbox ==
    //document.getElementById(category + "box").checked = true;
}

// == hides all markers of a particular category, and ensures the checkbox is cleared ==
function hide(category) {
    for (var i = 0; i < gmarkers.length; i++) {
        if (gmarkers[i].category == category && !gmarkers[i].isChecked) {
            gmarkers[i].setVisible(false);
        }
    }
    // == clear the checkbox ==
    //document.getElementById(category + "box").checked = false;
    // == close the info window, in case its open on a marker that we just hid
    infowindow.close();
}

function boxclick(isChecked, category) {
    if (!isChecked) {
        show(category);
    } else {
        hide(category);
    }
    // == rebuild the side bar
    //makeSidebar();
}

function displayInfo(widget) {
    var info = document.getElementById('info');
    info.innerHTML = 'Position: ' + widget.get('position') + ', distance: ' +
      widget.get('distance');
}

DistanceWidget.prototype = new google.maps.MVCObject();

function DistanceWidget() {

    this.set('map', gmap);
    this.set('position', gmap.getCenter());

    // Bind the marker map property to the DistanceWidget map property
    gmarker.bindTo('map', this);

    // Bind the marker position property to the DistanceWidget position
    // property
    gmarker.bindTo('position', this);

    // Create a new radius widget
    var radiusWidget = new RadiusWidget();

    // Bind the radiusWidget map to the DistanceWidget map
    radiusWidget.bindTo('map', this);

    // Bind the radiusWidget center to the DistanceWidget position
    radiusWidget.bindTo('center', this, 'position');

    // Bind to the radiusWidgets' distance property
    this.bindTo('distance', radiusWidget);

    // Bind to the radiusWidgets' bounds property
    this.bindTo('bounds', radiusWidget);

    //var me = this;
    //google.maps.event.addListener(marker, 'dblclick', function () {
    //    // When a user double clicks on the icon fit to the map to the bounds
    //    map.fitBounds(me.get('bounds'));
    //});
}

function RadiusWidget() {
    var circle = new google.maps.Circle({
        strokeWeight: 2,
        clickable: true
    });

    google.maps.event.addListener(circle, 'click', function () {
        contextMenu.hide();
    });
    // Set the distance property value, default to 10km.
    this.set('distance', 10);

    // Bind the RadiusWidget bounds property to the circle bounds property.
    this.bindTo('bounds', circle);

    // Bind the circle center to the RadiusWidget center property
    circle.bindTo('center', this);

    // Bind the circle map to the RadiusWidget map
    circle.bindTo('map', this);

    // Bind the circle radius property to the RadiusWidget radius property
    circle.bindTo('radius', this);

    RadiusWidget.prototype.center_changed = function () {
        var bounds = this.get('bounds');

        // Bounds might not always be set so check that it exists first.
        if (bounds) {
            var lng = bounds.getNorthEast().lng();

            // Put the sizer at center, right on the circle.
            var position = new google.maps.LatLng(this.get('center').lat(), lng);
            this.set('sizer_position', position);
        }
    };
}
RadiusWidget.prototype = new google.maps.MVCObject();

/**
* Update the radius when the distance has changed.
*/
RadiusWidget.prototype.distance_changed = function () {
    this.set('radius', this.get('distance') * 1000);
};
RadiusWidget.prototype.addSizer_ = function () {
    var sizer = new google.maps.Marker({
        draggable: true,
        title: 'Drag me!'
    });

    sizer.bindTo('map', this);
    sizer.bindTo('position', this, 'sizer_position');
    var me = this;
    google.maps.event.addListener(sizer, 'drag', function () {
        // Set the circle distance (radius)
        me.setDistance();
    });
};

RadiusWidget.prototype.distanceBetweenPoints_ = function (p1, p2) {
    if (!p1 || !p2) {
        return 0;
    }

    var R = 6371; // Radius of the Earth in km
    var dLat = (p2.lat() - p1.lat()) * Math.PI / 180;
    var dLon = (p2.lng() - p1.lng()) * Math.PI / 180;
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(p1.lat() * Math.PI / 180) * Math.cos(p2.lat() * Math.PI / 180) *
      Math.sin(dLon / 2) * Math.sin(dLon / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d;
};

/**
 * Set the distance of the circle based on the position of the sizer.
 */
RadiusWidget.prototype.setDistance = function () {
    // As the sizer is being dragged, its position changes.  Because the
    // RadiusWidget's sizer_position is bound to the sizer's position, it will
    // change as well.
    var pos = this.get('sizer_position');
    var center = this.get('center');
    var distance = this.distanceBetweenPoints_(center, pos);

    // Set the distance property for any objects that are bound to it
    this.set('distance', distance);

};

$(document).ready(function () {
    //$(document).on('click', '#InputsWrapper li', function (e) {
    //    if (e.button == 2) {
    //        var str = $(this).attr('id');
    //        var n = str.split("-");
    //        console.log(n[1]);
    //    }
    //});
});


$(document).ready(function () {
    //InitialScrollBar();
    //$("#placesbox").niceScroll({ autohidemode: true });
    //$("#HistoryWrapper").niceScroll({ autohidemode: true });

    //$.get('@Url.Action("TravelListPartial", "Map")', function (data) {
    //    $('#TravelListPartialView').html(data);
    //});
    //$.get('@Url.Action("TravelListPlacePartial", "Map")', function (data) {
    //    $('#TravelListPlacePartialView').html(data);
    //});
    //$.get('@Url.Action("SuggestPlacePartial", "Map")', function (data) {
    //    $('#SuggestTab').html(data);
    //});
    //$.get('@Url.Action("HistoryPartial", "Map")', function (data) {
    //    $('#HistoryTab').html(data);
    //});


    loadPartialView($('#TravelListPartialView'), '/Map/TravelListPartial');
    loadPartialView($('#TravelListPlacePartialView'), '/Map/TravelListPlacePartial');
    loadPartialView($('#SuggestTab'), '/Map/SuggestPlacePartial');
    loadPartialView($('#HistoryTab'), '/Map/HistoryPartial');

    CheckHistory();

    var $currentPes = $('<div class="center">資料讀取中... 0 %</div>');

    $.ajax({
        type: 'POST',
        url: '/Map/GetMap',
        dataType: 'json',
        cache: true,
        async: true,
        beforeSend: function () { console.log('carry on'); blockUI($currentPes); },
        success: function (data) {
            console.log('done');
            initialize(data);
            $.unblockUI();
        },
        error: function () {
            console.log('Load Error Occur !');
        },
        progress: function (e) {
            if (e.lengthComputable) {
                var pct = (e.loaded / e.total) * 100;
                var cPct = parseInt((e.loaded / e.total * 100), 10);
                $currentPes.html('資料讀取中... ' + cPct + ' %');
                //$progressBar.find('progress-bar').css('width', cPct);
                //$progressBar.find('progress-bar').css('width', parseInt((e.loaded / e.total * 100), 10)).html(parseInt((e.loaded / e.total * 100), 10));

            } else {
                console.warn('Content Length not reported!');
            }
        }
    });

    $("#searchView-text").keyup(function (e) {
        if (e.keyCode == 13) {
            //$(this).trigger("enterKey");
            //var str = $(this).attr('id');
            //var n = str.split(".");
            var name = $("#searchView-text").val();
            for (var i = 0; i < gmarkers.length; i++) {
                if (gmarkers[i].title == name) {
                    id = i;
                }
            }
            google.maps.event.trigger(gmarkers[id], "click");
            gmarkers[id].setVisible(true);
        }
    });

    $(".filterClass .btn").click(function (event) {
        boxclick($(this).hasClass("active"), $(this).val());
    });

    $(document).on('click', '.dropdown-menu .inner >li', function (e) {
        var $this = $(this).attr("rel");
        var selectedValue = $("#TravelListSelectPicker option:eq('" + $this + "')");
        var str = selectedValue.attr('value');
        var n = str.split("-");
        selectedList = n[1];
        $.ajax({
            url: '/Map/OnChangeTravelList',
            type: "POST",
        data: JSON.stringify({ selectedList: selectedList }), //if you need to post Model data, use this
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#TravelListPlacePartialView").html(data);

            for (var i = 0; i < gmarkers.length; i++) {
                if (gmarkers[i].isChecked) {
                    gmarkers[i].isChecked = false;
                }
            }
        }
    });
});

$(document).on('input', '#TravelListName', function (e) {
    var $this = $(this).val();
    if ($this != '') {
        $("#Create-List-Button").attr("disabled", false);
    }
    else {
        $("#Create-List-Button").attr("disabled", true);
    }
});

$('#History').click(function () {
    var test = $.cookie('wayptsCookie');
    console.log(test)
    $.ajax({
        url: '/Map/RetrieveHistory',
        type: "POST",
    data: JSON.stringify({ jsonHistory: test }), //if you need to post Model data, use this
    contentType: 'application/json; charset=utf-8',
    success: function (data) {
        $("#HistoryTab").html(data);
    }
});
});

$('#MapTabs a[href="#TravelTab"]').click(function () {
    //HideAllScrollBar();
    HideInputWrapper();
    ShowInputWrapper();
});

$('#MapTabs a[href="#SuggestTab"]').click(function () {
    //$("#placesbox").getNiceScroll().hide();
    HideInputWrapper();
    //HideAllScrollBar();
    //$("#InputsWrapper").getNiceScroll().hide();
    //$("#HistoryWrapper").getNiceScroll().hide();
    //$("#otherWrapper").getNiceScroll().show();
    //$("#FavorWrapper").getNiceScroll().show();
});

$('#MapTabs a[href="#HistoryTab"]').click(function () {
    //$("#placesbox").getNiceScroll().hide();
    HideInputWrapper();
    //HideAllScrollBar();
    //$("#InputsWrapper").getNiceScroll().hide();
    //$("#HistoryWrapper").getNiceScroll().resize();
    //$("#HistoryWrapper").getNiceScroll().show();

    //$("#otherWrapper").getNiceScroll().hide();
    //$("#FavorWrapper").getNiceScroll().hide();
    //$("#InputsWrapper").getNiceScroll().hide();
    //$("#HistoryInner").getNiceScroll().show();
});

});

function CheckHistory() {
    if ($.cookie('wayptsCookie') != null) {
        waypts = JSON.parse($.cookie('wayptsCookie'));
        console.log("Get Cookie!!");
        console.log(waypts);
    }
}

function HideAllScrollBar() {
    $("#otherWrapper").getNiceScroll().hide();
    $("#FavorWrapper").getNiceScroll().hide();
    $("#InputsWrapper").getNiceScroll().hide();
    $("#HistoryInner").getNiceScroll().hide();
    console.log("hide");
}

function HideInputWrapper() {
    $("#placesbox").getNiceScroll().hide();
    $("#placesbox").getNiceScroll().resize();
}
function ShowInputWrapper() {
    $("#placesbox").getNiceScroll().show();
    $("#placesbox").getNiceScroll().resize();
}

function SetGmarkerVisible(index) {
    google.maps.event.trigger(gmarkers[index], "click");
    gmarkers[index].setVisible(true);
}

function blockUI($input)
{
    var msg = ($input) ? $input : "資料讀取中...";
    $.blockUI({
        message: msg,
        fadeIn: 700,
        fadeOut: 700,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}
