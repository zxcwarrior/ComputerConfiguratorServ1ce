//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComputerConfiguratorService.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BuildKeyboards
    {
        public int BuildKeyboardID { get; set; }
        public int BuildID { get; set; }
        public int KeyboardID { get; set; }
    
        public virtual Builds Builds { get; set; }
        public virtual Keyboards Keyboards { get; set; }
    }
}
