(function () {
    function getHtmlString(elementId) {
        var originalElement = document.getElementById(elementId);
        var clonedElement = originalElement.cloneNode(true);
        // set original dimensions in case element is dynamically sized
        var originalHeight = originalElement.offsetHeight;
        var originalWidth = originalElement.offsetWidth;
        clonedElement.setAttribute('style', 'width: ' + originalWidth + 'px; height: ' + originalHeight + 'px;');
        var serializer = new XMLSerializer();
        
        return serializer.serializeToString(clonedElement);
    }
    
    function createCssUrl(path) {
        return location.protocol + '//' + location.host + '/' + path;
    }
    
    function createBody(form) {
        var width = form.elements['width'].value;
        var height = form.elements['height'].value;
        var filename = form.elements['filename'].value;
        
        return {
            content: getHtmlString('export'),
            cssUrls: [createCssUrl('Content/bootstrap.min.css')],
            height: height,
            width: width, 
            filename: filename
        };
    }
    
    document.getElementById('export-form').addEventListener('submit', function (e) {
        e.preventDefault();
        var submitButton = document.getElementById('export-submit');
        submitButton.innerText = 'Exporting...';
        submitButton.disabled = true;
        
        var xhr = new XMLHttpRequest();
        var body = createBody(e.target);
        
        xhr.open('POST', '/api/export/dom-to-image', true);
        xhr.responseType = 'blob';
        xhr.setRequestHeader('Content-type', 'application/json');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var contentDispositionHeader = xhr.getResponseHeader('Content-Disposition');
                    var fileName = contentDispositionHeader.match(/filename=(.*)/)[1];
                    // FileSaver.saveAs
                    saveAs(xhr.response, fileName);
                }
                submitButton.innerText = 'Export';
                submitButton.disabled = false;
            }
        };
        xhr.send(JSON.stringify(body));        
    });

    Highcharts.chart('chart-container', {

        chart: {
            type: 'column'
        },

        title: {
            text: ''
        },

        xAxis: {
            categories: ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },

        yAxis: {
            allowDecimals: false,
            min: 0,
            title: {
                text: 'Number of fruits'
            }
        },

        tooltip: {
            formatter: function () {
                return '<b>' + this.x + '</b><br/>' +
                    this.series.name + ': ' + this.y + '<br/>' +
                    'Total: ' + this.point.stackTotal;
            }
        },

        plotOptions: {
            column: {
                stacking: 'normal'
            }
        },

        series: [{
            name: 'John',
            data: [5, 3, 4, 7, 2],
            stack: 'male'
        }, {
            name: 'Joe',
            data: [3, 4, 4, 2, 5],
            stack: 'male'
        }, {
            name: 'Jane',
            data: [2, 5, 6, 2, 1],
            stack: 'female'
        }, {
            name: 'Janet',
            data: [3, 0, 4, 4, 3],
            stack: 'female'
        }]
    });
})();