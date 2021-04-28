
// 예상치 못한 에러 처리.
process.on('uncaughtException', (err) => {
    console.log('예기치 못한 에러', err);
});

// TCP server.
const net = require("net");
const server = net.createServer();
server.listen(9000, function () {
    console.log("server listening to 9000 port, %j", server.address());
});

// 문자열 바이트 길이 구하는 함수.
function strByteLength(s, b, i, c) {
    for (b = i = 0; c = s.charCodeAt(i++); b += c >> 11 ? 3 : c >> 7 ? 2 : 1);
    return b
}




// 본문.
var clients = {};

server.on("connection", function (socket) {
    try {
        let date = new Date();
        console.log("new client ", date);

        var recvData = Buffer.alloc(0);
        socket.on("data", async function (data) {
            try {
                for (var username in clients) { // 다른 유저들에게 전송.
                    if (clients[username] != socket) {
                        clients[username].write(data.toString());
                    }
                }

                var arr = [recvData, data];
                recvData = Buffer.concat(arr);
                if(recvData.byteLength > 4){
                    var byteCount = recvData.toString("utf8",0,4);

                    if(recvData.byteLength >= 4 + parseInt(byteCount)){
                        var originData = recvData.toString("utf8",4, 4 + parseInt(byteCount));

                        HandleData(originData);

                        recvData = recvData.slice(4 + parseInt(byteCount),recvData.byteLength);
                    }
                }

            } catch (error) {
                let date = new Date();
                console.error(date, ":", error);
            }
        });

        function HandleData(dataStrings) {
            var data = dataStrings.split("Partition");

            for (var index in data) {
                if (data[index] == "")
                    continue;

                Data = JSON.parse(data[index]);

                if (Data.eN == "Connection") {
                    if (clients.hasOwnProperty(Data.pN)) {
                        socket.destroy();
                        return;
                    }

                    clients[Data.pN] = socket;
                    console.log(clients);
                }
            }
        }

        socket.once("close", function (c) {
            try {
                var disconUserName = "";
                for (var username in clients) {
                    if (clients[username] == socket) {
                        disconUserName = username;
                    }
                }
                if (disconUserName == "")
                    return;

                var json = {
                    pN: disconUserName,
                    eN: "Disconnection",
                    eD: {}
                };

                var sendMsg = JSON.stringify(json)+ "Partition";

                var lngBuf = Buffer.alloc(4);
                var lng = strByteLength(sendMsg);
                if(lng)

                lngBuf.write(lng+"","utf8");

                for (var username in clients) { // 다른 유저들에게 전송.
                    if (clients[username] != socket) {
                        clients[username].write(lngBuf.toString() + sendMsg);
                    }
                }

                delete clients[disconUserName];

                let date = new Date();
                console.log("closed ", date);
                console.log(clients);

            } catch (error) {
                let date = new Date();
                console.error(date, ":", error);
            }
        });

    } catch (error) {
        let date = new Date();
        console.error(date, ":", error);
    }
});