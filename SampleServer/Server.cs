﻿using System;
using System.Collections.Generic;

using MiniUDP;

internal class Server
{
  private const int BUFFER_SIZE = 2048;

  private int port;
  private NetSocket netSocket;
  private Clock updateClock;

  // I/O buffer for reading and writing packet data
  private byte[] buffer;

  public Server(int port, double tickRate = 0.02)
  {
    this.port = port;
    this.buffer = new byte[BUFFER_SIZE];

    this.netSocket = new NetSocket();
    this.netSocket.Connected += this.OnConnected;
    this.netSocket.Disconnected += this.OnDisconnected;
    this.netSocket.TimedOut += this.OnTimedOut;

    this.updateClock = new Clock(tickRate);
    updateClock.OnFixedUpdate += this.OnFixedUpdate;
  }

  public void Start()
  {
    this.netSocket.Bind(this.port);
    this.updateClock.Start();
  }

  public void Update()
  {
    this.updateClock.Tick();
  }

  public void Stop()
  {
    this.netSocket.Shutdown();
    this.netSocket.Transmit();
  }

  private void OnFixedUpdate()
  {
    this.netSocket.Poll();
    this.netSocket.Transmit();
  }

  private void OnConnected(NetPeer peer)
  {
    Console.WriteLine("Connected: " + peer.ToString() + " (" + this.netSocket.PeerCount + ")");
    peer.MessagesWaiting += this.OnPeerMessagesWaiting;
  }

  private void OnDisconnected(NetPeer peer)
  {
    Console.WriteLine("Disconnected: " + peer.ToString() + " (" + this.netSocket.PeerCount + ")");
  }

  void OnTimedOut(NetPeer peer)
  {
    Console.WriteLine("Timed Out: " + peer.ToString() + " (" + this.netSocket.PeerCount + ")");
  }

  private void OnPeerMessagesWaiting(NetPeer source)
  {
    foreach (int length in source.ReadReceived(this.buffer))
    {
      byte sequence = this.buffer[9];
      Console.WriteLine("Received " + sequence + " from " + source.ToString());
      source.EnqueueSend(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, sequence }, 10);
    }
  }
}