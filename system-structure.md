CẤU  TRÚC HỆ THỐNG


SCMS.Backend/
│
├── SCMS.DomainEntities/      # Layer Domain: Định nghĩa các thực thể nghiệp vụ (Entities, Enums)
│   ├── Entities/            # Product, Order, User, ...
│   └── Enums/               # OrderStatus, PaymentMethod, ...
│
├── SCMS.Contracts/           # Layer Contracts: Định nghĩa các interface, DTO, contract giữa các layer
│   ├── Interfaces/          # IProductService, IOrderRepository, ...
│   └── DTOs/                # Chứa các thư mục DTO cho từng domain (Product, Order, ...)
│
├── SCMS.DAL/                 # Layer Data Access: Truy cập dữ liệu, context, repository
│   ├── Context/             # WatchStoreDbContext.cs
│   └── Repositories/        # ProductRepository, OrderRepository, ...
│
├── SCMS.BusinessService/     # Layer Business Logic: Xử lý nghiệp vụ
│   └── Services/            # ProductService, OrderService, ...
│
└── SCMS.WebWatchAPI/         # Layer Presentation: API đầu cuối
    ├── Controllers/         # AuthController, UserController, Admin, ...
    ├── Program.cs           # Entry point
    ├── appsettings.json     # Cấu hình
    └── wwwroot/             # Tài nguyên tĩnh


  

    SCMS.Frontend/
│
├── public/                  # Tài nguyên tĩnh
├── src/
│   ├── pages/               # Các trang chính (admin, brand, category, order, product, user, voucher...)
│   ├── components/          # Các component dùng chung và theo module
│   ├── services/            # Giao tiếp API backend (brand, product, user, ...)
│   ├── types/               # Định nghĩa kiểu dữ liệu TypeScript cho từng domain
│   ├── styles/              # CSS theo module
│   ├── utils/               # Tiện ích chung (api.ts ...)
│   └── ...                  # hooks, context (nếu có)
├── package.json             # Quản lý phụ thuộc
├── vite.config.js           # Cấu hình Vite
└── ...
```
