using System;
using _02._Scripts.Items;
using UnityEngine;

public static class ItemEffectFactory
{
    public static IItemEffect CreateItemEffect(ItemSO itemSo)
    {
        return itemSo.ItemType switch
        {
            ItemType.FlashLight => new FlashlightEffect(),
            ItemType.ChessBoard => new ChessBoardEffect(),
            ItemType.Camera     => new CameraEffect(Camera.main),
            ItemType.Key        => new FinalKeyEffect(),
            _                   => null
        };
    }
}