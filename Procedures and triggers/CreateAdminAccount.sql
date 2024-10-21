use BookingServices
INSERT INTO AspNetUsers (
    Id,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount
)
VALUES (
    'admin2024',
    'admin2@gmail.com',
    'ADMIN2@GMAIL.COM',
    'admin2@gmail.com',
    'ADMIN2@GMAIL.COM',
    0,
    'AQAAAAIAAYagAAAAEPF+gIomw00idLiy3HsZy1oDrZCGmuxbrr8BDUlPnG8cizssuAPdAqsMYdBrA3Li0g==', -- password is => Admin123@
    'RandomSecurityStampHere',
    'RandomConcurrencyStampHere',
    NULL,
    0,
    0,
    NULL,
    1,
    0
);

INSERT INTO AspNetUserRoles (UserId,RoleId)VALUES ('admin2024','Admin');