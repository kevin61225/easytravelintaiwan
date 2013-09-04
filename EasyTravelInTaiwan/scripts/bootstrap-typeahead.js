$("#Search").typeahead({
    source: function (query, process) {
        var bookList = [];
        //bookArray = {};

        // This is going to make an HTTP post request to the controller
        return $.post('/Search/SearchPlace', { query: query }, function (data) {

            // Loop through and push to the array
            $.each(data, function (i, book) {
                //bookArray[book.entity] = book;
                console.log(book);
                bookList.push(book.entity);
            });

            // Process the details
            process(bookList);
        });
    },
    /*
    updater: function (item) {
        //var selectedBook = bookArray[item].entity;

        // Set the text to our selected id
        //$("#details").text("Selected : " + selectedBook);
        return item;
    }
    */
});