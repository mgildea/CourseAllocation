function SPIN() {
    $(document.body).modal_spin();
}

//jQuery extension
$.fn.modal_spin = function () {
    var opts = {
        lines: 11, // The number of lines to draw
        length: 23, // The length of each line
        width: 8, // The line thickness
        radius: 40, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 9, // The rotation offset
        color: '#FFF', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 50, // Afterglow percentage
        shadow: true, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: '50%', // Top position relative to parent in px
        left: '50%' // Left position relative to parent in px
    }

    this.each(function () {
        var $this = $(this),
        data = $this.data();

        if (data.spinner) {
            data.spinner.stop();
            delete data.spinner;
            $("#spin_modal_overlay").remove();
            return this;
        }

        var spinElem = this;
        $('body').append('<div id="spin_modal_overlay" style="background-color: rgba(0, 0, 0, 0.6); width:100%; height:100%; position:fixed; top:0px; left:0px; z-index:' + (opts.zIndex - 1) + '"/>');
        spinElem = $("#spin_modal_overlay")[0];
        data.spinner = new Spinner($.extend({ color: $this.css('color') }, opts)).spin(spinElem);
    });
    return this;
};