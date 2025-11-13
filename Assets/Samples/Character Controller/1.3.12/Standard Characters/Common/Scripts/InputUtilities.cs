using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct FixedInputEvent
{
    private byte _wasEverSet;
    private uint _lastSetTick;

    public void Set(uint tick)
    {
        _lastSetTick = tick;
        _wasEverSet = 1;
    }

    public bool IsSet(uint tick)
    {
        if (_wasEverSet == 1)
        {
            return tick == _lastSetTick;
        }

        return false;
    }
}

public struct FixedInputState {
	private byte _wasEverDown;
	private uint _lastDownTick;
	private byte _wasEverUp;
	private uint _lastUpTick;

	public void Down(uint tick) {
		_lastDownTick = tick;
		_wasEverDown = 1;
	}

    public void Up(uint tick) {
		_lastUpTick = tick;
		_wasEverUp = 1;
	}

	public bool IsDown(uint tick) {
		if (_wasEverDown == 1) {
			return tick == _lastDownTick;
		}

		return false;
	}

	public bool IsUp(uint tick) {
		if (_wasEverUp == 1) {
			return tick == _lastUpTick;
		}

		return false;
	}
}