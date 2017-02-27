var OneTrueError;
(function (OneTrueError) {
    ;
    var Dataset = (function () {
        function Dataset() {
        }
        return Dataset;
    }());
    OneTrueError.Dataset = Dataset;
    ;
    var LineData = (function () {
        function LineData() {
        }
        return LineData;
    }());
    OneTrueError.LineData = LineData;
    var LineChart = (function () {
        function LineChart(targetElementOrId) {
            this.chart = null;
            this.lineChart = null;
            this.initChartGlobals();
            if (typeof targetElementOrId === "string") {
                var elem = document.getElementById(targetElementOrId);
                if (!elem)
                    throw new Error("Failed to find " + targetElementOrId);
                this.ctx = elem.getContext("2d");
            }
            else {
                if (!targetElementOrId)
                    throw new Error("Is not a HTMLElement: " + targetElementOrId);
                this.ctx = targetElementOrId.getContext("2d");
            }
            this.chart = new Chart(this.ctx);
        }
        LineChart.prototype.mergeOptions = function (obj1, obj2) {
            for (var p in obj2) {
                try {
                    // Property in destination object set; update its value.
                    if (obj2[p].constructor == Object) {
                        obj1[p] = this.mergeOptions(obj1[p], obj2[p]);
                    }
                    else {
                        obj1[p] = obj2[p];
                    }
                }
                catch (e) {
                    // Property in destination object not set; create it and set its value.
                    obj1[p] = obj2[p];
                }
            }
            return obj1;
        };
        LineChart.prototype.render = function (data) {
            if (this.lineChart !== null)
                this.lineChart.destroy();
            if (data.datasets.length) {
            }
            for (var j = 0; j < data.datasets.length; j++) {
                data.datasets[j] = this.mergeOptions(data.datasets[j], LineChart.LineThemes[j]);
            }
            var allEmpty;
            data.datasets.forEach(function (dataset) {
                dataset.data.forEach(function (item) {
                    if (item !== 0) {
                        allEmpty = false;
                        return;
                    }
                });
            });
            //if (allEmpty || data.datasets.length === 0) {
            //    this.ctx.font = "20px Arial";
            //    this.ctx.fillStyle = 'white';
            //    this.ctx.fillText("No reports have been received during the specified period.", 10, 50);
            //    return;
            //}
            this.lineChart = this.chart.Line(data, Chart.DefaultOptions);
        };
        LineChart.prototype.initChartGlobals = function () {
            Chart.defaults.global.scaleLineColor = "#eee";
            Chart.defaults.global.scaleFontColor = "#eee";
            Chart.defaults.global.responsive = true;
            Chart.DefaultOptions = {
                ///Boolean - Whether grid lines are shown across the chart
                scaleShowGridLines: true,
                //String - Colour of the grid lines
                scaleGridLineColor: "rgba(255,255,255,.05)",
                //Number - Width of the grid lines
                scaleGridLineWidth: 1,
                //Boolean - Whether to show horizontal lines (except X axis)
                scaleShowHorizontalLines: true,
                //Boolean - Whether to show vertical lines (except Y axis)
                scaleShowVerticalLines: false,
                //Boolean - Whether the line is curved between points
                bezierCurve: true,
                //Number - Tension of the bezier curve between points
                bezierCurveTension: 0.2,
                //Boolean - Whether to show a dot for each point
                pointDot: true,
                //Number - Radius of each point dot in pixels
                pointDotRadius: 4,
                //Number - Pixel width of point dot stroke
                pointDotStrokeWidth: 1,
                //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
                pointHitDetectionRadius: 20,
                //Boolean - Whether to show a stroke for datasets
                datasetStroke: true,
                //Number - Pixel width of dataset stroke
                datasetStrokeWidth: 2,
                //Boolean - Whether to fill the dataset with a colour
                datasetFill: false,
                //String - A legend template
                legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span class=\"glyphicon glyphicon-stop\" style=\"background-color:<%=datasets[i].strokeColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
            };
        };
        LineChart.LineThemes = [
            {
                fillColor: "rgba(245,254,22,0.2)",
                strokeColor: "#ffda70",
                pointColor: "#ffda70",
                pointStrokeColor: "#ffda70",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)"
            },
            {
                fillColor: "rgba(221,19,165,0.2)",
                strokeColor: "#ff9570",
                pointColor: "#ff9570",
                pointStrokeColor: "#ff9570",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)"
            },
            {
                fillColor: "rgba(255,82,22,0.2)",
                strokeColor: "#ff5216",
                pointColor: "#ff5216",
                pointStrokeColor: "#ff5216",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)"
            },
            {
                fillColor: "rgba(255,152,22,0.2)",
                strokeColor: "#ffc216",
                pointColor: "#ffc216",
                pointStrokeColor: "#ffc216",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)",
            },
            {
                fillColor: "rgba(255,187,22,0.2)",
                strokeColor: "#ff9816",
                pointColor: "#ff9816",
                pointStrokeColor: "#ff9816",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)",
            },
            {
                fillColor: "rgba(255,82,22,0.2)",
                strokeColor: "#ff5216",
                pointColor: "#ff5216",
                pointStrokeColor: "#ff5216",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(255,255,255,0.5)",
            }
        ];
        return LineChart;
    }());
    OneTrueError.LineChart = LineChart;
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=ChartViewModel.js.map