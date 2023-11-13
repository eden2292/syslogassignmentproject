﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using SyslogAssignmentProject.Interfaces;
using static Globals;
using Microsoft.AspNetCore.Http;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for UDP connections and updating the
  /// livefeed with new syslog messages.
  /// </summary>
  public class UdpSyslogReceiver
  {
    public IPEndPoint SourceIpAddress { get; private set; }
    private UdpClient _udpListener;
    public CancellationTokenSource TokenToStopSource;
    private CancellationToken _stopListening;

    private void RefreshListener()
    {
      _udpListener = new UdpClient(S_ReceivingPortNumber);
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }
    public bool CheckListener(int portNumber)
    {
      bool _valid = true;
      try
      {
        UdpClient _listener = new UdpClient(portNumber);
        _listener.Close();
      }
      catch
      {
        _valid = false;
      }
      return _valid;
    }
    public async Task StartListening()
    {
      RefreshListener();
      UdpReceiveResult _result = new UdpReceiveResult();
      while (!_stopListening.IsCancellationRequested)
      {
        try
        {
          Console.WriteLine(_stopListening.CanBeCanceled.ToString());
          _result = await _udpListener.ReceiveAsync(_stopListening);
          _ = Task.Run(() => HandleMessageAsync(_result.RemoteEndPoint, _result.Buffer));
        }
        catch(Exception ex)
        {
          break;
        }
      }
      _udpListener.Close();
      StartListening();
    }

    private async Task HandleMessageAsync(IPEndPoint clientEndpoint, byte[] message)
    {
      if (S_ListeningOptions.Equals("Both") || S_ListeningOptions.Equals("UDP"))
      {
        try
        {
          SourceIpAddress = clientEndpoint;
          SyslogMessage _formattedMessage;
          S_RadioList.UpdateList(new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "UDP"));
          _formattedMessage = new SyslogMessage(SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now,
            Encoding.ASCII.GetString(message), "UDP");
          if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) &&
            !_stopListening.IsCancellationRequested)
          {
            S_LiveFeedMessages.UpdateList(_formattedMessage);
          }
        }
        catch
        { }
      }
      return;
    }
  }
}
