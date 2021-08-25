
var lastDataTable;
$(document).ready(function () {
    // initialize datatable
    initializeDataTable('.az-datatable');
});

//Start chart 
$(document).ready(function () {
    // initialize datepicker
    // Class definition

    /* defined datas */
    var dataTargetProfit = [
        [1354586000000, 153],
        [1364587000000, 658],
        [1374588000000, 198],
        [1384589000000, 663],
        [1394590000000, 801],
        [1404591000000, 1080],
        [1414592000000, 353],
        [1424593000000, 749],
        [1434594000000, 523],
        [1444595000000, 258],
        [1454596000000, 688],
        [1464597000000, 364]
    ]
    var dataProfit = [
        [1354586000000, 53],
        [1364587000000, 65],
        [1374588000000, 98],
        [1384589000000, 83],
        [1394590000000, 980],
        [1404591000000, 808],
        [1414592000000, 720],
        [1424593000000, 674],
        [1434594000000, 23],
        [1444595000000, 79],
        [1454596000000, 88],
        [1464597000000, 36]
    ]
    var dataSignups = [
        [1354586000000, 647],
        [1364587000000, 435],
        [1374588000000, 784],
        [1384589000000, 346],
        [1394590000000, 487],
        [1404591000000, 463],
        [1414592000000, 479],
        [1424593000000, 236],
        [1434594000000, 843],
        [1444595000000, 657],
        [1454596000000, 241],
        [1464597000000, 341]
    ]
    var dataSet1 = [
        [0, 10],
        [100, 8],
        [200, 7],
        [300, 5],
        [400, 4],
        [500, 6],
        [600, 3],
        [700, 2]
    ];
    var dataSet2 = [
        [0, 9],
        [100, 6],
        [200, 5],
        [300, 3],
        [400, 3],
        [500, 5],
        [600, 2],
        [700, 1]
    ];

    $(document).ready(function () {




        /* flot toggle example */
        var flot_toggle = function () {

            var data = [
                {
                    label: "Target Profit",
                    data: dataTargetProfit,
                    color: color.info._400,
                    bars:
                    {
                        show: true,
                        align: "center",
                        barWidth: 30 * 30 * 60 * 1000 * 80,
                        lineWidth: 0,
                        /*fillColor: {
                            colors: [color.primary._500, color.primary._900]
                        },*/
                        fillColor:
                        {
                            colors: [
                                {
                                    opacity: 0.9
                                },
                                {
                                    opacity: 0.1
                                }]
                        }
                    },
                    highlightColor: 'rgba(255,255,255,0.3)',
                    shadowSize: 0
                },
                {
                    label: "Actual Profit",
                    data: dataProfit,
                    color: color.warning._500,
                    lines:
                    {
                        show: true,
                        lineWidth: 2
                    },
                    shadowSize: 0,
                    points:
                    {
                        show: true
                    }
                },
                {
                    label: "User Signups",
                    data: dataSignups,
                    color: color.success._500,
                    lines:
                    {
                        show: true,
                        lineWidth: 2
                    },
                    shadowSize: 0,
                    points:
                    {
                        show: true
                    }
                }]

            var options = {
                grid:
                {
                    hoverable: true,
                    clickable: true,
                    tickColor: '#f2f2f2',
                    borderWidth: 1,
                    borderColor: '#f2f2f2'
                },
                tooltip: true,
                tooltipOpts:
                {
                    cssClass: 'tooltip-inner',
                    defaultTheme: false
                },
                xaxis:
                {
                    mode: "time"
                },
                yaxes:
                {
                    tickFormatter: function (val, axis) {
                        return "$" + val;
                    },
                    max: 1200
                }

            };

            var plot2 = null;

            function plotNow() {
                var d = [];
                $("#js-checkbox-toggles").find(':checkbox').each(function () {
                    if ($(this).is(':checked')) {
                        d.push(data[$(this).attr("name").substr(4, 1)]);
                    }
                });
                if (d.length > 0) {
                    if (plot2) {
                        plot2.setData(d);
                        plot2.draw();
                    }
                    else {
                        plot2 = $.plot($("#flot-toggles"), d, options);
                    }
                }

            };

            $("#js-checkbox-toggles").find(':checkbox').on('change', function () {
                plotNow();
            });
            plotNow()
        }
        flot_toggle();
        /* flot toggle example -- end*/

        /* flot area */
        var flotArea = $.plot($('#flot-area'), [
            {
                data: dataSet1,
                label: 'New Customer',
                color: color.success._200
            },
            {
                data: dataSet2,
                label: 'Returning Customer',
                color: color.info._200
            }],
            {
                series:
                {
                    lines:
                    {
                        show: true,
                        lineWidth: 2,
                        fill: true,
                        fillColor:
                        {
                            colors: [
                                {
                                    opacity: 0
                                },
                                {
                                    opacity: 0.5
                                }]
                        }
                    },
                    shadowSize: 0
                },
                points:
                {
                    show: true,
                },
                legend:
                {
                    noColumns: 1,
                    position: 'nw'
                },
                grid:
                {
                    hoverable: true,
                    clickable: true,
                    borderColor: '#ddd',
                    tickColor: '#ddd',
                    aboveData: true,
                    borderWidth: 0,
                    labelMargin: 5,
                    backgroundColor: 'transparent'
                },
                yaxis:
                {
                    tickLength: 1,
                    min: 0,
                    max: 15,
                    color: '#eee',
                    font:
                    {
                        size: 0,
                        color: '#999'
                    }
                },
                xaxis:
                {
                    tickLength: 1,
                    color: '#eee',
                    font:
                    {
                        size: 10,
                        color: '#999'
                    }
                }

            });
        /* flot area -- end */

        var flotVisit = $.plot('#flotVisit', [
            {
                data: [
                    [3, 0],
                    [4, 1],
                    [5, 3],
                    [6, 3],
                    [7, 10],
                    [8, 11],
                    [9, 12],
                    [10, 9],
                    [11, 12],
                    [12, 8],
                    [13, 5]
                ],
                color: color.success._200
            },
            {
                data: [
                    [1, 0],
                    [2, 0],
                    [3, 1],
                    [4, 2],
                    [5, 2],
                    [6, 5],
                    [7, 8],
                    [8, 12],
                    [9, 9],
                    [10, 11],
                    [11, 5]
                ],
                color: color.info._200
            }],
            {
                series:
                {
                    shadowSize: 0,
                    lines:
                    {
                        show: true,
                        lineWidth: 2,
                        fill: true,
                        fillColor:
                        {
                            colors: [
                                {
                                    opacity: 0
                                },
                                {
                                    opacity: 0.12
                                }]
                        }
                    }
                },
                grid:
                {
                    borderWidth: 0
                },
                yaxis:
                {
                    min: 0,
                    max: 15,
                    tickColor: '#ddd',
                    ticks: [
                        [0, ''],
                        [5, '100K'],
                        [10, '200K'],
                        [15, '300K']
                    ],
                    font:
                    {
                        color: '#444',
                        size: 10
                    }
                },
                xaxis:
                {

                    tickColor: '#eee',
                    ticks: [
                        [2, '2am'],
                        [3, '3am'],
                        [4, '4am'],
                        [5, '5am'],
                        [6, '6am'],
                        [7, '7am'],
                        [8, '8am'],
                        [9, '9am'],
                        [10, '1pm'],
                        [11, '2pm'],
                        [12, '3pm'],
                        [13, '4pm']
                    ],
                    font:
                    {
                        color: '#999',
                        size: 9
                    }
                }
            });


    });

});
//end chart 

$(document).ready(function () {
    // initialize datepicker
    // Class definition

    var controls = {
        leftArrow: '<i class="fal fa-angle-left" style="font-size: 1.25rem"></i>',
        rightArrow: '<i class="fal fa-angle-right" style="font-size: 1.25rem"></i>'
    }

    var runDatePicker = function () {

        // minimum setup
        $('.az-datepicker-1').datepicker(
            {
                todayHighlight: true,
                orientation: "bottom left",
                templates: controls
            });


        // input group layout 
        $('.az-datepicker-2').datepicker(
            {
                todayHighlight: true,
                orientation: "bottom left",
                templates: controls,
                format: 'dd/MM/yyyy'
            });

        // input group layout for modal demo
        $('.az-datepicker-modal-2').datepicker(
            {
                todayHighlight: true,
                orientation: "bottom left",
                templates: controls
            });

        // enable clear button 
        $('.az-datepicker-3').datepicker(
            {
                todayBtn: "linked",
                clearBtn: true,
                todayHighlight: true,
                templates: controls,
                format: 'dd/mm/yyyy',
                autoclose: true

            });

        // enable clear button for modal demo
        $('.az-datepicker-modal-3').datepicker(
            {
                todayBtn: "linked",
                clearBtn: true,
                todayHighlight: true,
                templates: controls
            });

        // orientation 
        $('.az-datepicker-4-1').datepicker(
            {
                orientation: "top left",
                todayHighlight: true,
                templates: controls
            });

        $('.az-datepicker-4-2').datepicker(
            {
                orientation: "top right",
                todayHighlight: true,
                templates: controls
            });

        $('.az-datepicker-4-3').datepicker(
            {
                orientation: "bottom left",
                todayHighlight: true,
                templates: controls
            });

        $('.az-datepicker-4-4').datepicker(
            {
                orientation: "bottom right",
                todayHighlight: true,
                templates: controls
            });

        // range picker
        $('.az-datepicker-5').datepicker(
            {
                todayHighlight: true,
                templates: controls
            });

        // inline picker
        $('.az-datepicker-6').datepicker(
            {
                todayHighlight: true,
                templates: controls
            });
    }

    $(document).ready(function () {
        runDatePicker();
    });
});

$(document).ready(function () {
    $(function () {
        $('.select2').select2(
            {
                allowClear: true,
                placeholder: "Select ..."
            });
        $('.select2Edited').select2(
            {
                allowClear: false,
                placeholder: "Select Representative"
            });
        $('.multiSelect2').select2({
            tokenSeparators: [",", " "]
        });

        $(".select2-placeholder-multiple").select2(
            {
                placeholder: "Select State"
            });
        $(".js-hide-search").select2(
            {
                minimumResultsForSearch: 1 / 0
            });
        $(".js-max-length").select2(
            {
                maximumSelectionLength: 2,
                placeholder: "Select maximum 2 items"
            });
        $(".select2-placeholder").select2(
            {
                placeholder: "Select a state",
                allowClear: true
            });

        $(".js-select2-icons").select2(
            {
                minimumResultsForSearch: 1 / 0,
                templateResult: icon,
                templateSelection: icon,
                escapeMarkup: function (elm) {
                    return elm
                }
            });

        function icon(elm) {
            elm.element;
            return elm.id ? "<i class='" + $(elm.element).data("icon") + " mr-2'></i>" + elm.text : elm.text
        }

        $(".js-data-example-ajax").select2(
            {
                ajax:
                {
                    url: "https://api.github.com/search/repositories",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term, // search term
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        // parse the results into the format expected by Select2
                        // since we are using custom formatting functions we do not need to
                        // alter the remote JSON data, except to indicate that infinite
                        // scrolling can be used
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination:
                            {
                                more: (params.page * 30) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                placeholder: 'Search for a repository',
                escapeMarkup: function (markup) {
                    return markup;
                }, // let our custom formatter work
                minimumInputLength: 1,
                templateResult: formatRepo,
                templateSelection: formatRepoSelection
            });

        function formatRepo(repo) {
            if (repo.loading) {
                return repo.text;
            }

            var markup = "<div class='select2-result-repository clearfix d-flex'>" +
                "<div class='select2-result-repository__avatar mr-2'><img src='" + repo.owner.avatar_url + "' class='width-2 height-2 mt-1 rounded' /></div>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title fs-lg fw-500'>" + repo.full_name + "</div>";

            if (repo.description) {
                markup += "<div class='select2-result-repository__description fs-xs opacity-80 mb-1'>" + repo.description + "</div>";
            }

            markup += "<div class='select2-result-repository__statistics d-flex fs-sm'>" +
                "<div class='select2-result-repository__forks mr-2'><i class='fal fa-lightbulb'></i> " + repo.forks_count + " Forks</div>" +
                "<div class='select2-result-repository__stargazers mr-2'><i class='fal fa-star'></i> " + repo.stargazers_count + " Stars</div>" +
                "<div class='select2-result-repository__watchers mr-2'><i class='fal fa-eye'></i> " + repo.watchers_count + " Watchers</div>" +
                "</div>" +
                "</div></div>";

            return markup;
        }

        function formatRepoSelection(repo) {
            return repo.full_name || repo.text;
        }
    });
});

function refillDataTable(dataTableId, dataSource) {
    table = $('#' + dataTableId).DataTable();
    table.clear().destroy();
    dataSource();
}

function reinitializeDataTable(dataTableId) {
    initializeDataTable('#' + dataTableId);
}

function initializeDataTable(selector) {
    lastDataTable = $(selector).dataTable(
        {
            responsive: true,
            fixedHeader: true,
            lengthChange: true,
            ColReorder: true,
            dom:
                /*	--- Layout Structure 
                    --- Options
                    l	-	length changing input control
                    f	-	filtering input
                    t	-	The table!
                    i	-	Table information summary
                    p	-	pagination control
                    r	-	processing display element
                    B	-	buttons
                    R	-	ColReorder
                    S	-	Select

                    --- Markup
                    < and >				- div element
                    <"class" and >		- div with a class
                    <"#id" and >		- div with an ID
                    <"#id.class" and >	- div with an ID and a class

                    --- Further reading
                    https://datatables.net/reference/option/dom
                    --------------------------------------
                 */
                "<'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                /*{
                    extend:    'colvis',
                    text:      'Column Visibility',
                    titleAttr: 'Col visibility',
                    className: 'mr-sm-3'
                },*/
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    titleAttr: 'Generate PDF',
                    className: 'btn-outline-danger btn-sm mr-1',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                },
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    titleAttr: 'Generate Excel',
                    className: 'btn-outline-success btn-sm mr-1',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                },


                {
                    extend: 'print',
                    text: 'Print',
                    titleAttr: 'Print Table',
                    className: 'btn-outline-primary btn-sm',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }
            ]
        });
}
function initializeCustomDataTable(selector) {
    $(selector).dataTable(
        {
            responsive: true,
            fixedHeader: false,
            lengthChange: false,
            ColReorder: false,
            info: false,
            searching: false,
            paging: false,
            dom:
                /*	--- Layout Structure 
                    --- Options
                    l	-	length changing input control
                    f	-	filtering input
                    t	-	The table!
                    i	-	Table information summary
                    p	-	pagination control
                    r	-	processing display element
                    B	-	buttons
                    R	-	ColReorder
                    S	-	Select

                    --- Markup
                    < and >				- div element
                    <"class" and >		- div with a class
                    <"#id" and >		- div with an ID
                    <"#id.class" and >	- div with an ID and a class

                    --- Further reading
                    https://datatables.net/reference/option/dom
                    --------------------------------------
                 */
                "<'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                /*{
                    extend:    'colvis',
                    text:      'Column Visibility',
                    titleAttr: 'Col visibility',
                    className: 'mr-sm-3'
                },*/
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    titleAttr: 'Generate PDF',
                    className: 'btn-outline-danger btn-sm mr-1',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                },
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    titleAttr: 'Generate Excel',
                    className: 'btn-outline-success btn-sm mr-1',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }


                //{
                //    extend: 'print',
                //    text: 'Print',
                //    titleAttr: 'Print Table',
                //    className: 'btn-outline-primary btn-sm',
                //    exportOptions: {
                //        columns: "thead th:not(.noExport)"
                //    }
                //}
            ]
        });
}

function calculateTotal(sumElement, controlToReplace) {
    var total = 0;
    $('.' + sumElement).each(function () {
        if (this.innerHTML != 'null')
            total += parseFloat(this.innerHTML)
    });
    $('#' + controlToReplace).text(numberWithCommas(total));
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function columnNumberWithCommas(columnSelector) {
    $('.' + columnSelector).each(function () {
        var value = $(this).text().trim();
        $(this).text(numberWithCommas(value));
    });
}

function SetPaymentMethods() {
    var ContractMonths = Number($('#leasePeriodInMonthes').val());
    var TotalRent = Number($('#yearlyRent').val());
    $("#paymentMethod").select2({ placeholder: 'Select a payment method' });
    $("#paymentMethod").empty();
    $("#PNotes").empty();
    if (ContractMonths > 0 && TotalRent > 0) {
        var NoMethod = false;

        //if (TotalRent % ContractMonths == 0) {
        $("#paymentMethod").append('<option value="1">Every (1) Month</option>');
        $("#PNotes").append("<p class='text-info'> You can Pay Monthly . Number of installments =  " + ContractMonths + " </p>")
        NoMethod = false;
        //}

        // every 3
        if (ContractMonths % 3 == 0) {

            //if (TotalRent % (ContractMonths / 3) == 0) {
            $("#paymentMethod").append('<option value="2">Every (3) Three Months</option>');
            $("#PNotes").append("<p class='text-info'> You can Pay Every (3) Months . Number of installments =  " + (ContractMonths / 3) + " </p>")
            NoMethod = false;
            //}
        }

        // every 4
        if (ContractMonths % 4 == 0) {

            //if (TotalRent % (ContractMonths / 4) == 0) {
            $("#paymentMethod").append('<option value="3">Every (4) Four Months</option>');
            $("#PNotes").append("<p class='text-info'> You can Pay Every (4) Months . Number of installments =  " + (ContractMonths / 4) + " </p>")
            NoMethod = false;
            //}
        }

        // every 6
        if (ContractMonths % 6 == 0) {

            //if (TotalRent % (ContractMonths / 6) == 0) {
            $("#paymentMethod").append('<option value="4">Every (6) Six Months</option>');
            $("#PNotes").append("<p class='text-info'> You can Pay Every (6) Months . Number of installments =  " + (ContractMonths / 6) + " </p>")
            NoMethod = false;
            //}
        }

        // every 12
        if (ContractMonths % 12 == 0) {

            //if (TotalRent % (ContractMonths / 12) == 0) {
            $("#paymentMethod").append('<option value="5">Every (12) Twelve Months</option>');
            $("#PNotes").append("<p class='text-info'> You can Pay Every (12) Months . Number of installments =  " + (ContractMonths / 12) + " </p>")
            NoMethod = false;
            //}
        }

        //else {
        //    $("#PNotes").append("<p class='text-error'> Yearly rent amount is not dividable by installments count . installments count  </p>")
        //}

        if (NoMethod) {
            $("#PNotes").append("<p class='text-error'> Yearly rent amount is not dividable by installments count . Please change the contract period or the total amount  </p>")
        }
    }
    else {
        console.log("Values must be more than zero  ");
    }

}

monthDiff = function (dateFrom, dateTo) {
    //var months;
    //months = (dateTo.getFullYear() - dateFrom.getFullYear()) * 12;
    //months -= dateFrom.getMonth();
    //months += dateTo.getMonth();
    //return months <= 0 ? 0 : months;
    var daysDifference = Math.round((dateTo.getDate() - dateFrom.getDate()) / 30);

    return dateTo.getMonth() - dateFrom.getMonth() +
        (12 * (dateTo.getFullYear() - dateFrom.getFullYear())) + daysDifference;
};

function cutText(selector) {
    $(selector).each(function () {
        var text = "";
        if (this.innerHTML != 'null' && this.innerHTML != null && this.innerHTML != undefined) {
            text = this.innerHTML;
        }
        if (text.length > 40) {
            text = text.substring(1, 35);
            this.innerHTML = text + '...';
        }
    });
}

function initializeFilteredDataTable(selector, colFilter, ddlSelector) {
    $(selector).DataTable({
        responsive: true,
        fixedHeader: true,
        lengthChange: true,
        ColReorder: true,
        dom: "<'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        buttons: [
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                titleAttr: 'Generate PDF',
                className: 'btn-outline-danger btn-sm mr-1',
                footer: true,
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'excelHtml5',
                text: 'Excel',
                titleAttr: 'Generate Excel',
                className: 'btn-outline-success btn-sm mr-1',
                footer: true,
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'print',
                text: 'Print',
                titleAttr: 'Print Table',
                className: 'btn-outline-primary btn-sm',
                footer: true,
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            }
        ],
        initComplete: function () {
            this.api().columns(colFilter).every(function () {
                var column = this;
                var select = $('<select class="custom-select custom-select-sm"><option value="">Select Representative</option></select>')
                    .appendTo($(ddlSelector).empty())
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );
                        column.search(val ? '^' + val + '$' : '', true, false).draw();
                    });
                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>')
                });
                //$(select).select2();
            });

        }
    });
}

function initializeSelect(selector) {
    $(selector).select2();
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};
    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });
    return indexed_array;
}

function blockUI() {
    $.blockUI({
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
function unblockUI() {
    $.unblockUI();
}
