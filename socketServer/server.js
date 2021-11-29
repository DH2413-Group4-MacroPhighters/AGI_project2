const express = require('express');
const http = require('http');

const logger = require("morgan")
const path = require("path");
const socketHandler = require("./socketHandler.js");
const postHandler = require("./postHandler.js");

const CONFIG = require('../config.json');
const port = process.env.PORT || parseInt(CONFIG.PortServer);
const localIP = CONFIG.LocalIP;
const host = CONFIG.HostIP;
const portWs = CONFIG.PortWs;

const app = new express;
const server = http.createServer(app);




app.engine('html', require('ejs').renderFile);
app.set('view engine', 'html');
app.use(logger("short"))
app.get('/', (req, res) => {
    res.render(path.join(__dirname,  '..',  'frontEnd',  'index.html'), {host:host, port:portWs});
});
app.use(express.static(path.join(__dirname, '..', 'frontEnd'))); // This sends all static files in fr
// ontEnd upon request


postHandler.Start(app);
socketHandler.Start(port)

server.listen(port, () => {
    console.log('listening on http://' + localIP + ':' + port);
    console.log('Go to http://' + host + ':' + port);
});



