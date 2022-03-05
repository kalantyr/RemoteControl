package com.kalantyr.remotecontrol

import android.content.Context
import android.net.wifi.WifiManager
import android.os.Bundle
import android.text.format.Formatter
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.edit
import com.google.android.material.textfield.TextInputEditText
import java.util.*
import kotlin.time.Duration

class MainActivity : AppCompatActivity() {
    private lateinit var _tvDisplay: TextView
    private lateinit var _tvIP: TextView
    private lateinit var _remoteControlClient: IRemoteControlClient
    private lateinit var _btnCancel: Button
    private lateinit var editText_host: EditText

    private val KEY_HOST: String = "KEY_HOST"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        _remoteControlClient = RemoteControlClient()

        _tvDisplay = findViewById(R.id._tv)
        _tvIP = findViewById(R.id._tvIP)
        _btnCancel = findViewById(R.id.button_Cancel)

        editText_host = findViewById(R.id.editText_host)
        var host = getPreferences(Context.MODE_PRIVATE).getString(KEY_HOST, "http://10.0.3.3:55555")
        editText_host.setText(host.toString())

        showIP()

        val timerTask: TimerTask = object : TimerTask() {
            override fun run() {
                showPowerOffRemain()
            }
        }
        var timer = Timer()
        timer.schedule(timerTask, 100L, 5 * 1000L)

        findViewById<Button>(R.id.button_5min).setOnClickListener{ schedulePowerOff(Duration.parseIsoString("PT0H5M0S")) }
        findViewById<Button>(R.id.button_15min).setOnClickListener{ schedulePowerOff(Duration.parseIsoString("PT0H15M0S")) }
        findViewById<Button>(R.id.button_30min).setOnClickListener{ schedulePowerOff(Duration.parseIsoString("PT0H30M0S")) }
        findViewById<Button>(R.id.button_60min).setOnClickListener{ schedulePowerOff(Duration.parseIsoString("PT1H0M0S")) }
        _btnCancel.setOnClickListener { cancelPowerOff() }
    }

    fun showIP(){
        val wifiManager = applicationContext.getSystemService(Context.WIFI_SERVICE) as WifiManager
        val ipAddress: String = Formatter.formatIpAddress(wifiManager.connectionInfo.ipAddress)
        _tvIP.text = ipAddress
    }

    fun cancelPowerOff(){
        setHost()
        Thread {
            _remoteControlClient.cancelPowerOff()
            showPowerOffRemain()
        }.start()
    }

    fun schedulePowerOff(remain: Duration){
        setHost()
        Thread {
            _remoteControlClient.schedulePowerOff(remain)
            showPowerOffRemain()
        }.start()
    }

    fun showPowerOffRemain(){
        setHost()
        try {
            var remain = _remoteControlClient.getPowerOff()
            runOnUiThread {
                if (remain == null) {
                    _tvDisplay.text = "--"
                    _btnCancel.isEnabled = false
                } else {
                    _tvDisplay.text = "${remain.inWholeMinutes} мин"
                    _btnCancel.isEnabled = true
                }
            }
        }
        catch (ex: Exception) {
            val message = ex.message
            _tvDisplay.text = message
        }
    }

    private fun setHost() {
        val host = editText_host.text.toString()
        (_remoteControlClient as RemoteControlClient).host = host

        with (this.getPreferences(Context.MODE_PRIVATE).edit()) {
            putString(KEY_HOST, host)
            apply()
        }

//        val sharedPref = this.getPreferences(Context.MODE_PRIVATE).getString(KEY_HOST)
    }
}