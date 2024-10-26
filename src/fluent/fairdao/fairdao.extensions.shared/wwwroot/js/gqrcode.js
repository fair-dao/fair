import '/_content/fairdao.ui/js/zxing.js';
var QRCode = {
    Show(imgId, data) {
        const codeWriter = new ZXing.BrowserQRCodeSvgWriter();

        console.log(data);

        codeWriter.writeToDom('#' + imgId, data, 300, 300);
    }
};
export { QRCode }