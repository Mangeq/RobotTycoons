using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Money : MonoBehaviour
{
    public static List<Digit> money;
    private static char[] abc = " abcdefghijklmnopqrstuvwxyz".ToCharArray();
    public static List<string> letters;
    public static List<Digit> add;
    public static List<Digit> remove;
    private void Start()
    {
        letters = new List<string>();
        add = new List<Digit>()
        {
            new Digit (" ",900),
            new Digit ("a",900),
            new Digit ("b",900)
        }; 
        remove = new List<Digit>()
        {
            new Digit (" ",950),
            new Digit ("a",500),
            new Digit ("b",500)
        };
        foreach (char c in abc)
        {
            letters.Add(c.ToString());
        }
        MoneyAdd(add);
        MoneyRemove(remove);
    }
    void AddDigit(int index)
    {
        Digit.digits.Add(new Digit(letters[index + 1], 200));
    }
    static void MoneyAdd(List<Digit> addMoney)
    {
        foreach (Digit addDigit in addMoney) 
        {
            Digit moneyDigit = Digit.digits.Find(d => d.letter == addDigit.letter);
            int iAdd = addMoney.IndexOf(addDigit);
            if (moneyDigit == null)
            {
                Digit.digits.Add(new Digit(letters[iAdd], 0));
                moneyDigit = Digit.digits[iAdd];
            }
            if (moneyDigit.value + addDigit.value > 999)
            {
                int iMoney = Digit.digits.IndexOf(moneyDigit);
                if (iMoney + 1 >= Digit.digits.Count)
                {
                    Digit.digits.Add(new Digit(letters[iMoney + 1], 0));
                }
                addDigit.value -= 1000;
                moneyDigit.value += addDigit.value;
                Digit.digits[iMoney + 1].value++;
            }
            else 
            {
                moneyDigit.value += addDigit.value;
            }
        }
    }
    static void MoneyRemove(List<Digit> removeMoney)
    {
        foreach(Digit removeDigit in removeMoney)
        {
            Digit moneyDigit = Digit.digits.Find(d => d.letter == removeDigit.letter);
            if (moneyDigit.value - removeDigit.value < 0)
            {
                int i = Digit.digits.IndexOf(moneyDigit);
                if (Digit.digits[i + 1] != null && Digit.digits[i + 1].value != 0)
                {
                    Digit.digits[i + 1].value--;
                    moneyDigit.value += 1000;
                    moneyDigit.value -= removeDigit.value;
                }
            }
            else
            {
                moneyDigit.value -= removeDigit.value;
            }
        }
    }
    static void MoneyMultiplier(List<Digit> multiplierMoney, int multiplier)
    {
        foreach (Digit multiDigit in multiplierMoney)
        {
            if (multiplier * multiDigit.value > 1000000)
            {

            }
            else if(multiplier * multiDigit.value > 999 && multiplier * multiDigit.value < 1000000) 
            {
                int i = Digit.digits.IndexOf(multiDigit);
                if (Digit.digits[i + 1] == null)
                {
                    Digit.digits.Add(new Digit(letters[i + 1], 0));
                }
                int a = multiDigit.value * multiplier;
                if (a % 1000 != 0)
                { //2500
                    int b = a % 1000;//500
                    int c = a - b;//2000
                    int d = c / 1000;//2
                    multiDigit.value = b;
                    Digit.digits[i + 1].value = d;
                }
                else
                {//25.000
                    int b = a / 1000;//25
                    Digit.digits[i + 1].value = b;
                }
            }
            else
            {
                multiDigit.value *= multiplier;
            }
        }
    }

    public static void AddDigit(List<Digit> digits, int index)
    {
        digits.Add(new Digit(letters[index], 0));
    }
    public static long DigitToInt(List<Digit> digitList)
    {
        digitList.Reverse();
        for (int i = 0; i < digitList.Count; i++)
        {
            digitList[i - 1].value += (digitList[i].value * 1000);
        }
        return digitList[digitList.Count].value;
    }
    public static List<Digit> IntToDigit(int value)
    {
        List<Digit> digits = new List<Digit>();
        int i = 0;
        AddDigit(digits, i);
        for (int j = 0; j < digits.Count; j++)
        {
            Digit digit = digits[j];
            if (value > 1000)
            {
                AddDigit(digits, j + 1);
                if ((value & 1000) != 0)
                {
                    digit.value = value % 1000;
                    value /= 1000;
                }
                else
                {
                    value /= 1000;
                }
            }
            else
            {
                digit.value = value;
                value = 0;
            }
            i++;
        }
        return digits;
    }
    static string MoneyGet()
    {
        string money = "0";
        foreach (Digit digit in Digit.digits)
        {
            if (digit.value != 0)
            {
                money = (digit.value.ToString() +digit.letter);
            }
        }
        return money;
    }
}
public class Digit
{
    public string letter;  
    public static int digit = 1000;
    public int value = 0;
    public static List<Digit> digits = new List<Digit>();
    public Digit(string letter, int value)
    {
        this.letter = letter;
        this.value = value;
    }
}
