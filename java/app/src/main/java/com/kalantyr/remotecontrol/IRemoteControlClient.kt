package com.kalantyr.remotecontrol

import kotlin.time.Duration

interface IRemoteControlClient {
    fun getPowerOff(): Duration?

    fun cancelPowerOff()

    fun schedulePowerOff(remain: Duration)
}