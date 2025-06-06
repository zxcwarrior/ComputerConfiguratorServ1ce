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
    
    public partial class RAMs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RAMs()
        {
            this.BuildRAMs = new HashSet<BuildRAMs>();
        }
    
        public int RAMID { get; set; }
        public int ManufacturerID { get; set; }
        public string Model { get; set; }
        public int CapacityGB { get; set; }
        public int SpeedMHz { get; set; }
        public int RAMTypeID { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuildRAMs> BuildRAMs { get; set; }
        public virtual Manufacturers Manufacturers { get; set; }
        public virtual RAMTypes RAMTypes { get; set; }
    }
}
