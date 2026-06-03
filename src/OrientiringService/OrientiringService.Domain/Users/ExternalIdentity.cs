using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Domain.Users
{
    public class ExternalIdentity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; } // ссылка на моего локального пользователя

        public required string Provider{ get; set; } // кто аутентифицировал: например keycloak, google, yandex

        public required string Subject { get; set; } // sub claim из OIDC, то есть уникальный id пользователя у провайдера

        public string? Email { get; set; }

        public DateTime LinkedAt { get; set; }
    }
}
