using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Todo.Core;

namespace Todo.Data
{
    public class CryptoRepo : IScoped, IRepo
    {
        private readonly IDynamoDBContext _context;

        public CryptoRepo(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<List<DBRecord>> Load(string dbkey)
        {
            var id = BuildId(dbkey);
            var db = await _context.LoadAsync<DB>(id);
            if (db == null) return new List<DBRecord>();

            var (k, iv) = GetKeyAndIV(dbkey, db.Salt);

            return await Decrypt(db.Data, k, iv);
        }

        public async Task Save(string dbkey, IEnumerable<DBRecord> data)
        {
            var id = BuildId(dbkey);

            if (!data.Any())
            {
                await RemoveRecord();
                return;
            }

            var salt = GenerateSalt();
            var (k, iv) = GetKeyAndIV(dbkey, salt);

            var s = await Encrypt(data, k, iv);

            var db = new DB
            {
                DbKey = id,
                Data = s,
                Salt = salt,
                Updated = DateTime.Now,
            };

            await _context.SaveAsync(db);

            async Task RemoveRecord()
            {
                var tmp = new DB
                {
                    DbKey = id,
                };
                await _context.DeleteAsync(tmp);
            }
        }

        static async Task<List<DBRecord>> Decrypt(string input, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;


            await using var ms = new MemoryStream(Decode(input));
            await using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var lines = new List<DBRecord>();
            while (true)
            {
                var id = await sr.ReadLineAsync();
                if (id == null) break;

                var data = await sr.ReadLineAsync();
                lines.Add(new DBRecord { Id = id, Data = data });
            }

            return lines;
        }

        static async Task<string> Encrypt(IEnumerable<DBRecord> input, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            await using var ms = new MemoryStream();
            await using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            await using (var sw = new StreamWriter(cs))
            {
                foreach (var line in input)
                {
                    await sw.WriteLineAsync(line.Id);
                    await sw.WriteLineAsync(line.Data);
                }
            }

            return Encode(ms.ToArray());
        }

        static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();

            var buffer = new byte[SALT_LEN_BYTES];

            rng.GetBytes(buffer);

            return Encode(buffer);
        }

        const int SALT_LEN_BYTES = 16;
        const int IV_LEN_BYTES = 16;
        const int KEY_LEN_BYTES = 32;

        static (byte[], byte[]) GetKeyAndIV(string password, string salt)
        {
            var saltBytes = Decode(salt);
            using var pbkdf = new Rfc2898DeriveBytes(password, saltBytes, 10000);

            var key = pbkdf.GetBytes(KEY_LEN_BYTES);
            var iv = pbkdf.GetBytes(IV_LEN_BYTES);

            return (key, iv);
        }

        static string Encode(byte[] input) => Convert.ToBase64String(input);
        static byte[] Decode(string input) => Convert.FromBase64String(input);

        static string BuildId(string key)
        {
            var buffer = Encoding.UTF8.GetBytes(key);

            using var sha1 = SHA1.Create();
            var hashed = sha1.ComputeHash(buffer);

            return Encode(hashed);
        }
    }
}
