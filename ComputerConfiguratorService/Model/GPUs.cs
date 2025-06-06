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
    
    public partial class GPUs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GPUs()
        {
            this.Builds = new HashSet<Builds>();
        }
    
        public int GPUID { get; set; }
        public int ManufacturerID { get; set; }
        public int VendorID { get; set; }
        public string Model { get; set; }
        public int GPUMemoryTypeID { get; set; }
        public int MemoryGB { get; set; }
        public int CoreBaseClock { get; set; }
        public Nullable<int> CoreBoostClock { get; set; }
        public Nullable<int> MemoryClock { get; set; }
        public int GPULength { get; set; }
        public int PowerConsumption { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Builds> Builds { get; set; }
        public virtual GPUMemoryTypes GPUMemoryTypes { get; set; }
        public virtual Manufacturers Manufacturers { get; set; }
        public virtual Vendors Vendors { get; set; }
    }
}
