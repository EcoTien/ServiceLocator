﻿using UnityEngine;

namespace EcoMine.Service
{
    public interface IService
    {
        /// <summary>
        /// Register service to service locator.
        /// </summary>
        void RegisterService();
    }
}