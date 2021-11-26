const express = require('express');
const http = require('http');
const multer  = require('multer')
const logger = require("morgan")
const path = require("path");
const socketHandler = require("./socketHandler.js");

const CONFIG = require('../config.json');
const port = process.env.PORT || parseInt(CONFIG.PortServer);
const localIP = CONFIG.LocalIP;
const host = CONFIG.HostIP;
const portWs = CONFIG.PortWs;

const app = new express;
const server = http.createServer(app);


const upload = multer({
    dest:path.join(__dirname,  '..',  'frontEnd',  'uploads')

})


app.engine('html', require('ejs').renderFile);
app.set('view engine', 'html');
app.use(logger("short"))
app.get('/', (req, res) => {
    res.render(path.join(__dirname,  '..',  'frontEnd',  'index.html'), {host:host, port:portWs});
});
app.use(express.static(path.join(__dirname, '..', 'frontEnd'))); // This sends all static files in fr
// ontEnd upon request

const formidable = require ( 'formidable' );
const incomingForm = formidable.IncomingForm;

app.post ( '/mapPost', function ( req, res ) {
    let form = new incomingForm ();
    form.uploadDir = path.join(__dirname,  '..',  'frontEnd',  'uploads'); // This is the directory you have to create manually.
    form.parse ( req, function ( err, fields, files ) {
        if ( err ) {
            console.log ( 'Some error: ', err );
        }
        console.log ( 'File saved' );
    } );
});

socketHandler.Start(port)

server.listen(port, () => {
    console.log('listening on http://' + localIP + ':' + port);
    console.log('Go to http://' + host + ':' + port);
});



