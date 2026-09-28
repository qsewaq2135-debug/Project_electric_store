using EBIKES09.Models;
using System.Collections.Generic;
using System.Linq;

public class SessionManager
{
    private static SessionManager _instance;
    private Dictionary<string, List<CartItem>> _temporaryCarts; 

    public static SessionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SessionManager();
            }
            return _instance;
        }
    }

    public bool IsLoggedIn { get; private set; }
    public int UserId { get; private set; }
    public string UserName { get; private set; }

    private SessionManager()
    {
        _temporaryCarts = new Dictionary<string, List<CartItem>>();
    }

    public void Login(int userId, string userName)
    {
        IsLoggedIn = true;
        UserId = userId;
        UserName = userName;
    }

    public void Logout()
    {
        IsLoggedIn = false;
        UserId = 0;
        UserName = null;
    }

    
    public List<CartItem> GetTemporaryCart(string sessionId)
    {
        if (_temporaryCarts.ContainsKey(sessionId))
        {
            return _temporaryCarts[sessionId];
        }
        return new List<CartItem>();
    }

    public void AddToTemporaryCart(string sessionId, CartItem cartItem)
    {
        if (!_temporaryCarts.ContainsKey(sessionId))
        {
            _temporaryCarts[sessionId] = new List<CartItem>();
        }

        var existingItem = _temporaryCarts[sessionId].FirstOrDefault(ci => ci.ProductId == cartItem.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += cartItem.Quantity; 
        }
        else
        {
            _temporaryCarts[sessionId].Add(cartItem);
        }
    }

    public void UpdateTemporaryCart(string sessionId, List<CartItem> cartItems)
    {
        if (_temporaryCarts.ContainsKey(sessionId))
        {
            _temporaryCarts[sessionId] = cartItems;
        }
        else
        {
            _temporaryCarts.Add(sessionId, cartItems);
        }
    }

    public void ClearTemporaryCart(string sessionId)
    {
        if (_temporaryCarts.ContainsKey(sessionId))
        {
            _temporaryCarts.Remove(sessionId); 
        }
    }
}