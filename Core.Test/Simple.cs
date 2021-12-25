using System;
using System.Collections.Generic;
using Core.Model;
using Xunit;
using Xunit.Abstractions;

namespace Core.Test
{
    public class Item
    {
        public int Type;
        public int Level;
    }
    public class Inventory
    {
        public List<Item> Items = new List<Item>();
        public int Level;
    }
    
    public class SocialInfo
    {
        public string Nickname;
        public int Level;
    }

    public class SimpleProfile : BaseViModel<SimpleProfile>
    {
        public ViModelRegion<Inventory> Inventory = new ViModelRegion<Inventory>();
        public ViModelRegion<SocialInfo> SocialInfo = new ViModelRegion<SocialInfo>();
    }
    
    public class Simple
    {
        private ITestOutputHelper _output;
        public Simple(ITestOutputHelper output)
        {
            _output = output;
            ViStorage.Initialize((level, message) => _output.WriteLine(message));
        }
        
        
        [Fact]
        public void Test1()
        {
            var profile = GetNewSimpleModel();
            
            _output.WriteLine($"\n\nBefore: {profile.GenerateJson(true)}");
            
            profile.ContinuouslyModifyRegions(() =>
            {
                profile.SocialInfo.ContinuousModify(model =>
                {
                    model.Level++;
                });
                
                profile.Inventory.ContinuousModify(model =>
                {
                    model.Items.RemoveAt(model.Items.Count - 1);
                });
            });
            
            _output.WriteLine($"\n\nAfter: {profile.GenerateJson(true)}");
        }

        private SimpleProfile GetNewSimpleModel()
        {
            var profile = new SimpleProfile();

            profile.ContinuouslyModifyRegions(() =>
            {
                profile.SocialInfo.ContinuousModify(model =>
                {
                    model.Nickname = "Player";
                });
                
                profile.Inventory.ContinuousModify(model =>
                {
                    model.Items.Add(new Item { Type = 0 });
                });
            });
            

            return profile;
        }
    }
}