using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Currency {

    public Action<Currency> m_onCurrencyUpdated;

    public Currency(int ammount)
    {
        m_ammount = ammount;
    }

    public Currency(int ammount, Action<Currency> onCurrencyUpdated = null)
    {
        m_ammount = ammount;
        m_onCurrencyUpdated = onCurrencyUpdated;
    }

    [SerializeField]
    public int m_ammount;
    public int Ammount
    {
        get { return m_ammount; }
    }

    public void SetAmmount(int ammount)
    {
        m_ammount = ammount;
        UpdateCurrency();
    }

    public void AddCurrency(int ammount)
    {
        m_ammount += ammount;
        UpdateCurrency();
    }
    public void AddCurrency(Currency currency)
    {
        m_ammount += currency.m_ammount;
        UpdateCurrency();
    }

    public void SubstractCurrency(int ammount)
    {
        m_ammount -= ammount;
        UpdateCurrency();
    }
    public void SubstractCurrency(Currency currency)
    {
        m_ammount -= currency.m_ammount;
        UpdateCurrency();
    }

    public bool CanAfford(Currency currency)
    {
        return m_ammount >= currency.m_ammount;
    }

    private void UpdateCurrency()
    {
        if (m_onCurrencyUpdated != null)
        {
            m_onCurrencyUpdated(this);
        }
    }

    //All fields must be of serializabe types (like int, float, string or another custom class with [Serializable])
}
