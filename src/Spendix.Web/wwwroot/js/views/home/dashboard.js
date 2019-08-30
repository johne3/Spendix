// Set new default font family and font color to mimic Bootstrap's default styling
//Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
//Chart.defaults.global.defaultFontColor = '#858796';

$(document).ready(function () {
    $('#bankAccountId').on('change', function () {
        window.location = window.location.origin + '?bankAccountId=' + $('#bankAccountId').val() + '&range=' + $('#range').val();
    });

    $('#range').on('change', function () {
        window.location = window.location.origin + '?bankAccountId=' + $('#bankAccountId').val() + '&range=' + $('#range').val();
    });


    // Pie Chart Example
    var ctx = document.getElementById("expensesByCategoryChart");
    var myPieChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: chartLabels,
            datasets: [{
                data: chartData,
                backgroundColor: chartColors,
                //backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                //hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                hoverBorderColor: "rgba(234, 236, 244, 1)"
            }]
        },
        options: {
            maintainAspectRatio: false,
            tooltips: {
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                caretPadding: 10,
            },
            legend: {
                display: true,
                position: 'right',
                labels: {
                    //generateLabels: function (chart) {
                    //    var data = chart.data;
                    //    if (data.labels.length && data.datasets.length) {
                    //        return data.labels.map(function (label, i) {
                    //            var meta = chart.getDatasetMeta(0);
                    //            //var style = meta.controller.getStyle(i);

                    //            var ds = data.datasets[0];
                    //            var arc = meta.data[i];
                    //            var style = arc && arc.custom || {};

                    //            return {
                    //                text: label,
                    //                fillStyle: style.backgroundColor,
                    //                strokeStyle: style.borderColor,
                    //                lineWidth: style.borderWidth,
                    //                hidden: isNaN(data.datasets[0].data[i]) || meta.data[i].hidden,

                    //                // Extra data used for toggling the correct item
                    //                index: i
                    //            };
                    //        });
                    //    }
                    //    return [];
                    //}
                    generateLabels: function (chart) {
                        var data = chart.data;
                        if (data.labels.length && data.datasets.length) {
                            return data.labels.map(function (label, i) {
                                var meta = chart.getDatasetMeta(0);
                                var ds = data.datasets[0];
                                var arc = meta.data[i];
                                var custom = arc && arc.custom || {};
                                var getValueAtIndexOrDefault = Chart.helpers.getValueAtIndexOrDefault;
                                var arcOpts = chart.options.elements.arc;
                                var fill = custom.backgroundColor ? custom.backgroundColor : getValueAtIndexOrDefault(ds.backgroundColor, i, arcOpts.backgroundColor);
                                var stroke = custom.borderColor ? custom.borderColor : getValueAtIndexOrDefault(ds.borderColor, i, arcOpts.borderColor);
                                var bw = custom.borderWidth ? custom.borderWidth : getValueAtIndexOrDefault(ds.borderWidth, i, arcOpts.borderWidth);

                                // We get the value of the current label
                                var value = chart.config.data.datasets[arc._datasetIndex].data[arc._index];

                                return {
                                    // Instead of `text: label,`
                                    // We add the value to the string
                                    text: label + " : " + value,
                                    fillStyle: fill,
                                    strokeStyle: stroke,
                                    lineWidth: bw,
                                    hidden: isNaN(ds.data[i]) || meta.data[i].hidden,
                                    index: i
                                };
                            });
                        } else {
                            return [];
                        }
                    }
                }
            },
            cutoutPercentage: 80    
        },
    });
});