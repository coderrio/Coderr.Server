
//export class ChartData {
//    labels: string[];
//    datasets: ChartDataSet[];
//}

//export class ChartDataSet {
//    label: string;
//    backgroundColor: string;
//    data: number[];
//    fill: boolean;
//    borderColor: string;
//}

//export class ChartTheme {
//    static colors: string[] = [
//        "#0094DA",
//        "#35281D",
//        "#7A82AB",
//        "#307473",
//        "#001242",
//        "#005E7C",
//        "#EE6352",
//        "#57A773",
//        "#ADBDFF",
//        "#34E5FF"
//    ];
//}

//export class LineChart {
//    private chart: any;

//    constructor(public title: string | null) {
//        if (this.title === '')
//            this.title = null;
//    }
//    render(data: ChartData) {
//        this.applyColors(data);

//        if (this.chart == null) {
//            this.create(data);
//        } else {
//            this.update(data);
//        }
//    }

//    private update(data: ChartData) {

//    }
//    private applyColors(data: ChartData) {
//        var index = 0;
//        data.datasets.forEach(item => {
//            if (item.backgroundColor == null) {
//                item.backgroundColor = ChartTheme.colors[index++];
//                item.borderColor = item.backgroundColor;
//            }
//            item.fill = false;
//        });
//    }

//    private create(data: ChartData) {
//        var canvas = <HTMLCanvasElement>document.getElementById("line-chart");
//        var ctx = canvas.getContext("2d");

//        this.chart = new Chart(ctx, {
//            type: 'line',
//            data: data,
//            //responsive: true,
//            options: {
//                maintainAspectRatio: false,
//                title: {
//                    display: this.title != null,
//                    text: this.title
//                },
//                scales:
//                {
//                    xAxes: [{
//                        gridLines: {
//                            display: false
//                        }
//                    }],
//                    yAxes: [{
//                        display: true,
//                        ticks: {
//                            //suggestedMin: 0,
//                            beginAtZero: true
//                        }
//                    }]
//                }
//            }
//        });
//    }
//}
