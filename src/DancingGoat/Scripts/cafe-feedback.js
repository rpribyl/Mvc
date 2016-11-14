$(".cafe-feedback-heading").click(function () {
    $header = $(this);

    // Get a reference to the form contents
    $content = $header.parent().find(".xs-hidden").first();

    // Toggle the visibility of the contents with a callback function
    $content.slideToggle(0, function () {
        $header.text(function () {
            return $content.is(":visible") ? "Collapse" : "Give feedback";
        });
    });
});
