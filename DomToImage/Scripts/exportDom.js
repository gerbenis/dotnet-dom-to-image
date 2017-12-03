var webpage = require('webpage');
var system = require('system');

function exit(err) {
    if (err) {
        system.stderr.writeLine(err);
    }
    phantom.exit(err ? 1 : 0);
}

function createDocument(options) {
    var cssElements = [];
    if (options.cssUrls && options.cssUrls.length) {
        cssElements = options.cssUrls.map(function (link) { return '<link href="' + link + '" rel="stylesheet">'; });
    }
    return '<!DOCTYPE html><html><head>' +
        cssElements.join('') +
        '</head><body style="background-color: transparent">' +
        options.content +
        '</body></html>';
}

function calculateScale(originalWidht, originalHeight, targetWidth, targetHeight) {
    var result = { x: 1, y: 1 };
    var scale;
    if (targetWidth && targetHeight) {
        result.x = targetWidth / originalWidht;
        result.y = targetHeight / originalHeight;
    } else if (targetWidth) {
        scale = targetWidth / originalWidht;
        result.x = scale;
        result.y = scale;
    } else if (targetHeight) {
        scale = targetHeight / originalHeight;
        result.x = scale;
        result.y = scale;
    }
    
    return result;
}

function getImageType(filename) {
    var extension = filename.split('.').pop().toLowerCase();
    var fileType = 'PNG';
    
    if (extension) {
        if (extension === 'JPG' || extension === 'JPEG') {
            fileType = 'JPEG'
        } else {
            fileType = extension.toUpperCase();
        }
    }
    
    return fileType;
}

function exportDom(options) {
    var page = webpage.create();
    page.content = createDocument(options);
    page.onLoadFinished = function (status) {
        if (status !== 'success') {
            exit('Failed to load content');
        }
        
        var dimensions = page.evaluate(function() {
            var elem = document.body.firstElementChild;
            return { width: elem.offsetWidth, height: elem.offsetHeight };
        });
        var scale = calculateScale(dimensions.width, dimensions.height, options.width, options.height);
        
        page.clipRect = {
            top: 0,
            left: 0,
            width: Math.round(dimensions.width * scale.x),
            height: Math.round(dimensions.height * scale.y)
        };
        page.evaluate(function (s) {
            var elem = document.body.firstElementChild;
            elem.setAttribute('style', elem.getAttribute('style') +
                ' -webkit-transform: scaleX(' + s.x + ') scaleY(' + s.y + '); -webkit-transform-origin: 0 0;');
            setTimeout(function () { window.callPhantom({ finished: true }); }, 0);
        }, scale);
    };
    page.onCallback = function (data) {
        if (data.finished) {
            var base64Image = page.renderBase64(getImageType(options.filename));
            system.stdout.writeLine(base64Image);
            exit();
        }
    };
}

try {
    var options = JSON.parse(system.args[1]);
    exportDom(options);
} catch (e) {
    exit(e);
}

