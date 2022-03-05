package com.kalantyr.remotecontrol

import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL
import kotlin.time.Duration

class RemoteControlClient: IRemoteControlClient {
    private var _host: String = "http://10.0.2.2:55555"

    override fun getPowerOff(): Duration? {
        val url = URL("$_host/power/off")
        var s = url.readText();
        if (s.isEmpty())
            return null;

        s = s.trim('"')
        var i = s.lastIndexOf('.')
        s = s.substring(0, i)

        // TODO: ну почему приходится так изголяться?
        var parts = s.split(':')
        val hh = parts[0]
        val mm = parts[1]
        val ss = parts[2]
        s = "PT${hh}H${mm}M${ss}S"
        return Duration.parseIsoString(s)
    }

    override fun cancelPowerOff() {
        val url = URL("$_host/power/off")
        val connection = url.openConnection() as HttpURLConnection
        with(connection) {
            //connection.setRequestProperty("Accept", "application/json")
            connection.setRequestProperty("Content-Type", "application/json")
            requestMethod = "POST"

            val wr = OutputStreamWriter(outputStream);
            wr.write("\"00:00:00\"");
            wr.flush();

            if (responseCode != 200)
                throw Exception(responseMessage)
        }
    }
}