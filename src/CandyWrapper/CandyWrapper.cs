using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public class CandyWrapper : IDisposable
{
    private static readonly object _syncRoot = new Object();
    private static volatile Dictionary<string, string[]> _moduleFields = new Dictionary<string, string[]>();
    private readonly string _session;
    private readonly string _username;
    private readonly string _password;
    private readonly sugarsoap _sugarsoap = new sugarsoap();

    public CandyWrapper(string url, string username, string password)
    {
        SetUrl(url);
        _username = username;
        _password = password;
        _session = Login(username, password);
    }

    private CandyWrapper()
    {
        //Do not want users to instatiate without parameters.
    }

    public string Session
    {
        get { return _session; }
    }

    #region IDisposable Members

    public void Dispose()
    {
        Logout(_session);
    }

    #endregion

    private string[] _GetModuleFields(string module)
    {
        string key = module;
        lock (_syncRoot)
        {
            if (_moduleFields.ContainsKey(key))
                return _moduleFields[key];
            string[] fields = GetModuleFields(module);
            _moduleFields.Add(key, fields);
        }
        return _moduleFields[key];
    }

    private static string ConvertToProper(string sValue)
    {
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sValue);
    }

    public string CreateAccount(string user, string pass, string name, string phone, string url)
    {
        return _sugarsoap.create_account(user, pass, name, phone, url);
    }

    public string CreateCase(string user, string pass, string name)
    {
        return _sugarsoap.create_case(user, pass, name);
    }

    public string CreateContact(string user, string pass, string sFirst, string sLast, string sEmail)
    {
        return _sugarsoap.create_contact(user, pass, sFirst, sLast, sEmail);
    }

    public string CreateLead(string user, string pass, string sFirst, string sLast, string sEmail)
    {
        return _sugarsoap.create_lead(user, pass, sFirst, sLast, sEmail);
    }

    public string CreateOpportunity(string user, string pass, string name, string sAmount)
    {
        return _sugarsoap.create_opportunity(user, pass, name, sAmount);
    }

    public string CreateSession(string user, string pass)
    {
        return _sugarsoap.create_session(user, pass);
    }

    public string EndSession(string user)
    {
        return _sugarsoap.end_session(user);
    }

    public string[] GetAvailableModules()
    {
        module_list list = _sugarsoap.get_available_modules(_session);
        error_value error = list.error;
        VerifySugarResult.Verify(error);
        int length = 1;
        if (list.modules.Length > 0)
        {
            length = list.modules.Length;
        }
        var strArray = new string[length];
        int index = 0;
        foreach (string str in list.modules)
        {
            strArray[index] = str;
            index++;
        }
        return strArray;
    }

    public SugarEntry GetEntry(string module, string id, string[] fields)
    {
        module = ConvertToProper(module);
        get_entry_result result = _sugarsoap.get_entry(_session, module, id, fields);
        VerifySugarResult.Verify(result.error);
        var entry = new SugarEntry(module);
        name_value[] valueList = result.entry_list[0].name_value_list;
        foreach (name_value value in valueList)
            entry.Add(value.name, value.value);
        return entry;
    }

    /// <summary>
    /// Gets all fields for module
    /// </summary>
    public SugarEntry GetEntry(string module, string id)
    {
        return GetEntry(module, id, _GetModuleFields(module));
    }
    public List<SugarEntry> GetEntryList(string module, string query, string order, int offset, int limit, int del) {
        return GetEntryList(module, query, order, offset, _GetModuleFields(module), limit, del);
    }

    public List<SugarEntry> GetEntryList(string module, string query, string order, int offset,
                                                         string[] fields, int limit, int del)
    {
        module = ConvertToProper(module);
        get_entry_list_result result = _sugarsoap.get_entry_list(_session, module, query, order, offset,
                                                                 fields, limit, del);
        VerifySugarResult.Verify(result.error);
        var entryList = new List<SugarEntry>();
        entry_value[] valueList1 = result.entry_list;
        foreach (entry_value value1 in valueList1)
        {
            name_value[] valueList2 = value1.name_value_list;
            var entry = new SugarEntry(module);
            foreach (name_value value2 in valueList2)
                entry.Add(value2.name, value2.value);
            entryList.Add(entry);
        }
        return entryList;
    }

    public string GetGMTTime()
    {
        return _sugarsoap.get_gmt_time();
    }

    public string[] GetModuleFields(string module)
    {
        module = ConvertToProper(module);
        module_fields fields = _sugarsoap.get_module_fields(_session, module);
        var strArray = new string[fields.module_fields1.Length];
        field[] fieldArray = fields.module_fields1;
        int index = 0;
        foreach (field field in fieldArray)
        {
            strArray[index] = field.name;
            index++;
        }
        return strArray;
    }

    public string[] GetNoteAttachment(string id)
    {
        note_attachment attachment2 = _sugarsoap.get_note_attachment(_session, id).note_attachment;
        return new[] {attachment2.file, attachment2.filename};
    }

    public string[] GetRelationships(string module, string id, string sRelMod,
                                     string sRelModQuery, int deleted)
    {
        module = ConvertToProper(module);
        get_relationships_result result = _sugarsoap.get_relationships(_session, module, id, sRelMod, sRelModQuery,
                                                                       deleted);
        VerifySugarResult.Verify(result.error);
        id_mod[] ids = result.ids;
        var strArray2 = new string[ids.Length];
        int index = 0;
        foreach (id_mod mod in ids)
        {
            strArray2[index] = mod.id;
            index++;
        }
        return strArray2;
    }

    public string GetServerTime()
    {
        return _sugarsoap.get_server_time();
    }

    public string GetServerVersion()
    {
        return _sugarsoap.get_server_version();
    }

    public string GetSugarFlavor()
    {
        return _sugarsoap.get_sugar_flavor();
    }

    public string GetUserId(string session)
    {
        return _sugarsoap.get_user_id(session);
    }

    public string GetUserTeamId(string session)
    {
        return _sugarsoap.get_user_team_id(session);
    }

    public int IsLoopback()
    {
        return _sugarsoap.is_loopback();
    }

    public int IsUserAdmin(string session)
    {
        return _sugarsoap.is_user_admin(session);
    }

    private string Login(string user, string pass)
    {
        return Login(user, pass, "1", string.Empty);
    }

    private string Login(string user, string pass, string version, string appName)
    {
        var auth = new user_auth();
        set_entry_result result;
        string str = GetMD5Hash(pass);
        string id;
        auth.user_name = user;
        auth.password = str;
        auth.version = version;
        result = _sugarsoap.login(auth, appName);
        VerifySugarResult.Verify(result.error);
        return result.id;
    }

    private string Logout(string session)
    {
        error_value errorValue = _sugarsoap.logout(session);
        string str = "Logout Successful!";
        if (errorValue.number != "0")
        {
            str = "Logout Error: ";
            str = (str + errorValue.number + "\n") + errorValue.description;
        }
        return str;
    }

    public string RelateRecord(string parent, string parentId, string child, string childId)
    {
        parent = ConvertToProper(parent);
        child = ConvertToProper(child);
        var value = new set_relationship_value();
        value.module1 = parent;
        value.module1_id = parentId;
        value.module2 = child;
        value.module2_id = childId;
        error_value value2 = _sugarsoap.set_relationship(_session, value);
        return (value2.number + ":" + value2.description);
    }

    public int SeamlessLogin(string session)
    {
        return _sugarsoap.seamless_login(session);
    }

    public List<SugarEntry> SearchByModule(string search, string[] modules, int offset, int limit)
    {
        var result =
            _sugarsoap.search_by_module(_username, this.GetMD5Hash(_password), search, modules, offset, limit);
        VerifySugarResult.Verify(result.error);
        var entryList = new List<SugarEntry>();
        entry_value[] valueList1 = result.entry_list;
        foreach (entry_value value1 in valueList1) {
            name_value[] valueList2 = value1.name_value_list;
            var entry = new SugarEntry(value1.module_name);
            foreach (name_value value2 in valueList2)
                entry.Add(value2.name, value2.value);
            entryList.Add(entry);
        }
        return entryList;
    }

    public string SetEntry(SugarEntry entry)
    {
        var valueArray = new name_value[entry.Count];
        int i = 0;
        foreach (var sugarEntry in entry)
        {
            valueArray[i] = new name_value();
            valueArray[i].name = sugarEntry.Key;
            valueArray[i].value = sugarEntry.Value;
            i++;
        }
        set_entry_result result = _sugarsoap.set_entry(_session, entry.Module, valueArray);
        VerifySugarResult.Verify(result.error);
        return result.id;
    }

    private string GetMD5Hash(string pass)
    {
        byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(pass));
        var builder = new StringBuilder();
        for (int i = 0; i < buffer.Length; i++)
        {
            builder.Append(buffer[i].ToString("x2"));
        }
        return builder.ToString();
    }

    private int SetUrl(string url)
    {
        var request = (HttpWebRequest) WebRequest.Create(url);
        var response = (HttpWebResponse) request.GetResponse();
        _sugarsoap.Url = url;
        return 1;
    }
}