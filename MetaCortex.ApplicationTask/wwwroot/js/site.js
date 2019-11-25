//ES5

window.API_DOMAIN = 'http://localhost:3264';

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function formatDateForApi(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [month, day, year].join('.');
}

function formatDateForChart(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [day, month, year].join('/');
}

Date.prototype.addDays = function(days) {
    return new Date(new Date(this.valueOf()).getTime() + (1000 * 60 * 60 * 24 * days));
}

var vueApp = new Vue({
    el: '#app',
    data: {
        startDate: null,
        endDate: null,
        today: null,
        chartData: null,
        exchangeRates: null,
        exchangeRateCode: null,
    },
    watch: {
        startDate: function(newValue, oldValue) {
            var newValueDate = new Date(newValue);

            if (newValueDate > new Date()) {
                this.startDate = formatDate(new Date());
                alert('startDate is cannot greater than today');
            }

            if (newValueDate > new Date(this.endDate)) {
                this.endDate = formatDate(newValueDate);
            }
        },
        endDate: function(newValue, oldValue) {
            var newValueDate = new Date(newValue);

            if (newValueDate > new Date()) {
                this.endDate = formatDate(new Date());
                alert('endDate is cannot greater than today');
            }

            if (new Date(this.startDate) > newValueDate) {
                this.endDate = formatDate(new Date(oldValue));
                alert('endDate is cannot less than startDate');
            }
        },
    },
    created: function() {
        this.onCreated();
    },
    computed: {
        formatedToday: function() {
            return formatDate(this.today);
        }
    },
    methods: {
        onCreated: function() {
            this.getExchangeRates();
            this.today = new Date();
            this.startDate = formatDate(this.today.addDays(-6));
            this.endDate = formatDate(this.today.addDays(-1));
        },
        getExchangeRates: function() {
            var self = this;
            var exchangeRates = localStorage.getItem('exchangeRates');
            if (exchangeRates) {
                exchangeRates = JSON.parse(exchangeRates);
                self.exchangeRates = exchangeRates;
            }
            else {
                var post = $.post(window.API_DOMAIN + '/Home/getExchangeRates');
                post.done(function(res) {
                    localStorage.setItem('exchangeRates', JSON.stringify(res.data[0].exchangeRates));
                    self.getExchangeRates();
                });
            }
        },
        getDataPost: function() {
            var self = this;

            if (self.startDate == null || self.endDate == null || self.exchangeRateCode == null || new Date(self.startDate) > new Date(self.endDate) || new Date(self.endDate) > new Date()) {
                var alertMessage = 'Check your data';

                if (self.startDate == null) alertMessage += ', startDate cannot be null';
                if (self.endDate == null) alertMessage += ', endDate cannot be null';
                if (self.exchangeRateCode == null) alertMessage += ', exchangeRateCode cannot be null';
                if (new Date(self.startDate) > new Date(self.endDate)) alertMessage += ', startDate cannot be greater than endDate';
                if (new Date(self.endDate) > new Date()) alertMessage += ', endDate cannot be greater than today';

                alertMessage += '.';

                alert(alertMessage);
                return;
            }

            //Localstorage||WebSQL cache for request can be included
            var post = $.post(window.API_DOMAIN + '/Home/getData?Code=' + self.exchangeRateCode + '&startDate=' + formatDateForApi(self.startDate) + '&endDate=' + formatDateForApi(self.endDate))
            post.fail(function(err) { console.error(err); })
            post.done(function(res) {
                var resDataLength = res.data.length;
                for (var i = 0; i < resDataLength; i++) res.data[i].date = res.data[i].date.split('T')[0];
                if (self.chartData != null) self.chartData = null;

                self.chartData = res;
                if (self.chartData != null) self.SetupCanvas();
            })
        },
        SetupCanvas: function() {
            var chartJsDays = [];
            var chartJsData = [];

            var chartJsAllData = [];

            var dayLength = this.chartData.data.length;

            var decimalLength = 0;

            for (var i = 0; i < dayLength; i++) {
                try {
                    var currentPrice = parseFloat(this.chartData.data[i].exchangeRates[0].price);

                    chartJsAllData.push({
                        x: this.chartData.data[i].date,
                        y: parseFloat(currentPrice)
                    });

                    chartJsData.push();
                    chartJsDays.push();
                } catch (e) { }
            }

            var elem = $('#exchangeRateChart');
            elem.show();
            var ctx = elem[0].getContext('2d');

            var labelName = '';
            var getTimeOut = 0;

            while (labelName.length < 1 || getTimeOut > 20) {
                try { labelName = this.chartData.data[0 + getTimeOut].exchangeRates[0].name; }
                catch (e) { }
                getTimeOut++;
            }


            var myChart = new Chart(ctx, {
                type: 'line',
                data: {
                    datasets: [{
                        label: labelName,
                        data: chartJsAllData,
                        backgroundColor: [
                            'rgba(255, 255, 255, 0)',
                        ],
                        borderColor: 'rgba(255, 99, 132, 1)'
                        ,
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        xAxes: [{
                            display: true,
                            type: 'time',
                            time: {
                                parser: 'YYYY-MM-DD',
                                tooltipFormat: 'll',
                                unit: 'day',
                                unitStepSize: 1,
                                displayFormats: {
                                    'day': 'DD.MM.YYYY'
                                }
                            },
                            scaleLabel: {
                                display: true,
                                labelString: 'Date'
                            }
                        }],
                        yAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Value'
                            }
                        }],
                    }
                }
            });
        }
    }
});