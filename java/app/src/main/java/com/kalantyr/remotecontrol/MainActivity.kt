package com.kalantyr.remotecontrol

import android.content.Context
import android.net.wifi.WifiManager
import android.os.Bundle
import android.text.format.Formatter
import android.widget.Button
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import java.util.*

class MainActivity : AppCompatActivity() {
    private lateinit var _tvDisplay: TextView
    private lateinit var _tvIP: TextView
    private lateinit var _remoteControlClient: IRemoteControlClient
    private lateinit var _btnCancel: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        _remoteControlClient = RemoteControlClient()

        _tvDisplay = findViewById(R.id._tv)
        _tvIP = findViewById(R.id._tvIP)
        _btnCancel = findViewById(R.id.button_Cancel)

        showIP()

        val timerTask: TimerTask = object : TimerTask() {
            override fun run() {
                showPowerOffRemain()
            }
        }
        var timer = Timer()
        timer.schedule(timerTask, 100L, 5 * 1000L)

        _btnCancel.setOnClickListener { cancelPowerOff() }
    }

    fun showIP(){
        val wifiManager = applicationContext.getSystemService(Context.WIFI_SERVICE) as WifiManager
        val ipAddress: String = Formatter.formatIpAddress(wifiManager.connectionInfo.ipAddress)
        _tvIP.text = ipAddress
    }

    fun cancelPowerOff(){
        Thread {
            _remoteControlClient.cancelPowerOff()
            showPowerOffRemain()
        }.start()
    }

    fun showPowerOffRemain(){
        try {
            var remain = _remoteControlClient.getPowerOff()
            runOnUiThread {
                if (remain == null) {
                    _tvDisplay.text = "--"
                    _btnCancel.isEnabled = false
                } else {
                    _tvDisplay.text = remain.toString()
                    _btnCancel.isEnabled = true
                }
            }
        }
        catch (ex: Exception) {
            val message = ex.message
            _tvDisplay.text = message
        }
    }
}