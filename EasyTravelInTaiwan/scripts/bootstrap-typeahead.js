$("#Search").typeahead({
    source: function (query, process) {
        var placeList = [];
        // This is going to make an HTTP post request to the controller
        return $.post('/Search/SearchPlace', { query: query }, function (data) {
            // Loop through and push to the array
            $.each(data, function (i, place) {
                placeList.push(place.entity);
            });
            // Process the details
            process(placeList);
        });
    }
    /*
    updater: function (item) {
        //var selectedBook = bookArray[item].entity;

        // Set the text to our selected id
        //$("#details").text("Selected : " + selectedBook);
        return item;
    }
    */
});