$(function () {
    var rate = $(".type-dropdown").find(":selected").data("rate");
    var weight = $(".weight-input").val();

    $(".computed-input").val(computedRate(rate, weight));

    function computedRate(rate, weight) {
        var computedRate = Math.round((rate * weight) * 100) / 100;
        return computedRate;
    }

    $(".type-dropdown").on("change", function () {
        //grab data-rate
        rate = $(this).find(":selected").data("rate");

        $(".computed-input").val(computedRate(rate, weight));
    });

    $(".weight-input").on("input", function () {
        weight = $(this).val();
        $(".computed-input").val(computedRate(rate, weight));
    });

    $(".description-input").on("input", function () {
        $(".computed-input").val(computedRate(rate, weight));
    });

});