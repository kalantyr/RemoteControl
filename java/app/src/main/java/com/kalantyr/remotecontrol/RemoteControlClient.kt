package com.kalantyr.remotecontrol

import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL
import kotlin.time.Duration

class RemoteControlClient: IRemoteControlClient {
    var host: String = ""

    override fun getPowerOff(): Duration? {
        val url = URL("$host/power/off")
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
        val url = URL("$host/power/off")
        val connection = url.openConnection() as HttpURLConnection
        with(connection) {
            connection.setRequestProperty("Content-Type", "application/json")
            requestMethod = "POST"

            val wr = OutputStreamWriter(outputStream);
            wr.write("\"00:00:00\"");
            wr.flush();

            if (responseCode != 200)
                throw Exception(responseMessage)
        }
    }

    override fun schedulePowerOff(remain: Duration) {
        val url = URL("$host/power/off")
        val connection = url.openConnection() as HttpURLConnection
        with(connection) {
            connection.setRequestProperty("Content-Type", "application/json")
            requestMethod = "POST"

            var hh = remain.inWholeHours
            var mm = remain.inWholeMinutes - remain.inWholeHours * 60
            var ss = remain.inWholeSeconds - remain.inWholeMinutes * 60
            var body = "${toStr(hh)}:${toStr(mm)}:${toStr(ss)}"
            body = "\"$body\""

            val wr = OutputStreamWriter(outputStream);
            wr.write(body);
            wr.flush();

            if (responseCode != 200)
                throw Exception(responseMessage)
        }
    }

    private fun toStr(value: Long): String {
        if (value < 9)
            return "0$value"
        else
            return "" + value
    }
}