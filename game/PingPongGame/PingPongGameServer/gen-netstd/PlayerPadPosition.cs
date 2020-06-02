/**
 * Autogenerated by Thrift Compiler (0.13.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thrift;
using Thrift.Collections;

using Thrift.Protocol;
using Thrift.Protocol.Entities;
using Thrift.Protocol.Utilities;
using Thrift.Transport;
using Thrift.Transport.Client;
using Thrift.Transport.Server;
using Thrift.Processor;



public partial class PlayerPadPosition : TBase
{
  private int _playerId;
  private Position _position;

  public int PlayerId
  {
    get
    {
      return _playerId;
    }
    set
    {
      __isset.playerId = true;
      this._playerId = value;
    }
  }

  public Position Position
  {
    get
    {
      return _position;
    }
    set
    {
      __isset.position = true;
      this._position = value;
    }
  }


  public Isset __isset;
  public struct Isset
  {
    public bool playerId;
    public bool position;
  }

  public PlayerPadPosition()
  {
  }

  public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
  {
    iprot.IncrementRecursionDepth();
    try
    {
      TField field;
      await iprot.ReadStructBeginAsync(cancellationToken);
      while (true)
      {
        field = await iprot.ReadFieldBeginAsync(cancellationToken);
        if (field.Type == TType.Stop)
        {
          break;
        }

        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I32)
            {
              PlayerId = await iprot.ReadI32Async(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 2:
            if (field.Type == TType.Struct)
            {
              Position = new Position();
              await Position.ReadAsync(iprot, cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          default: 
            await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            break;
        }

        await iprot.ReadFieldEndAsync(cancellationToken);
      }

      await iprot.ReadStructEndAsync(cancellationToken);
    }
    finally
    {
      iprot.DecrementRecursionDepth();
    }
  }

  public async Task WriteAsync(TProtocol oprot, CancellationToken cancellationToken)
  {
    oprot.IncrementRecursionDepth();
    try
    {
      var struc = new TStruct("PlayerPadPosition");
      await oprot.WriteStructBeginAsync(struc, cancellationToken);
      var field = new TField();
      if (__isset.playerId)
      {
        field.Name = "playerId";
        field.Type = TType.I32;
        field.ID = 1;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteI32Async(PlayerId, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      if (Position != null && __isset.position)
      {
        field.Name = "position";
        field.Type = TType.Struct;
        field.ID = 2;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await Position.WriteAsync(oprot, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      await oprot.WriteFieldStopAsync(cancellationToken);
      await oprot.WriteStructEndAsync(cancellationToken);
    }
    finally
    {
      oprot.DecrementRecursionDepth();
    }
  }

  public override bool Equals(object that)
  {
    var other = that as PlayerPadPosition;
    if (other == null) return false;
    if (ReferenceEquals(this, other)) return true;
    return ((__isset.playerId == other.__isset.playerId) && ((!__isset.playerId) || (System.Object.Equals(PlayerId, other.PlayerId))))
      && ((__isset.position == other.__isset.position) && ((!__isset.position) || (System.Object.Equals(Position, other.Position))));
  }

  public override int GetHashCode() {
    int hashcode = 157;
    unchecked {
      if(__isset.playerId)
        hashcode = (hashcode * 397) + PlayerId.GetHashCode();
      if(__isset.position)
        hashcode = (hashcode * 397) + Position.GetHashCode();
    }
    return hashcode;
  }

  public override string ToString()
  {
    var sb = new StringBuilder("PlayerPadPosition(");
    bool __first = true;
    if (__isset.playerId)
    {
      if(!__first) { sb.Append(", "); }
      __first = false;
      sb.Append("PlayerId: ");
      sb.Append(PlayerId);
    }
    if (Position != null && __isset.position)
    {
      if(!__first) { sb.Append(", "); }
      __first = false;
      sb.Append("Position: ");
      sb.Append(Position== null ? "<null>" : Position.ToString());
    }
    sb.Append(")");
    return sb.ToString();
  }
}

