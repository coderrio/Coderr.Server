$(function() {
    $("[data-val-required]:not(:hidden):not(:checkbox)").after('<span class="required-indicator">*</span>');
    //$(".icon-info-sign[title], [data-title]").tooltip();

});

function momentsAgo(dateStr) {
    if (dateStr.substr(dateStr.length - 1, 1) !== 'Z') {
        dateStr = dateStr + 'Z'
    }
    return moment(dateStr).fromNow();
}

function nl2br(str, is_xhtml) {
    var breakTag = is_xhtml ? "<br />" : "<br>";
    return (str + "").replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, "$1" + breakTag + "$2");
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf("?") + 1).split("&");
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split("=");
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

var queryParameters = getUrlVars();
if (queryParameters["usernote"]) {
    var p = queryParameters["usernote"];
    console.log(p);
    var a = decodeURIComponent(p);
    console.log(a);
    humane.log(a);
}

var ChartService = {
    ColorThemes: [
        { Red: 56, Green: 64, Blue: 66 },
        { Red: 0, Green: 148, Blue: 218 }
    ],
    CreateColor: function(themeIndex) {
        var color = window.ChartService.ColorThemes[themeIndex];
        return {
            fillColor: "rgba(" + color.Red + "," + color.Green + "," + color.Blue + ",0.2)",
            strokeColor: "rgba(" + color.Red + "," + color.Green + "," + color.Blue + ",0.8)"
        };
    },
    Build: function(graphId, datasets, options) {
        var mergedDataSet = [];
        var legendData = [];
        var index = 0;
        var self = this;
        $.each(datasets,
            function(d, item) {
                legendData.push(item.legend);
                var colors = ChartService.CreateColor(index);
                mergedDataSet.push({
                    fillColor: colors.fillColor,
                    strokeColor: colors.strokeColor,
                    data: item.data,
                    title: item.legend
                });

                index = index + 1;
            });

        for (var i = 0; i < options.labels.x.length; i++) {
            if (i % 2 === 0)
                options.labels.x[i] = "";
        }


        var graphOptions = {
            pointDot: false,
            pointDotRadius: 0,
            pointDotStrokeWidth: 0,
            scaleOverride: false,
            scaleShowGridLines: false,
            scaleOverlay: false,
            datasetStroke: false,
            datasetFill: false
        };

        var ctx = $("#" + graphId)[0].getContext("2d");
        new Chart(ctx).Line({ labels: options.labels.x, datasets: mergedDataSet }, graphOptions);
        $("#" + graphId).css("width", "800px");
        $("#" + graphId).css("height", "200px");
        var theLegend = document.getElementById(graphId + "Legend");
        if (theLegend) {
            legend(theLegend, mergedDataSet);
        }

    }
};