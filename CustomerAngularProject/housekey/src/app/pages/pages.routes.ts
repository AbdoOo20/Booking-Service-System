import { Routes } from "@angular/router";
import { PagesComponent } from "./pages.component";
import { RegisterComponent } from "./register/register.component";

export const routes: Routes = [
    {
        path: "",
        component: PagesComponent,
        children: [
            {
                path: "",
                loadComponent: () =>
                    import("./home/home.component").then(
                        (c) => c.HomeComponent
                    ),
            },
            {
                path: "home",
                loadComponent: () =>
                    import("./home/home.component").then(
                        (c) => c.HomeComponent
                    ),
            },
            {
                path: "about",
                loadComponent: () =>
                    import("./about/about.component").then(
                        (c) => c.AboutComponent
                    ),
            },
            {
                path: "contact",
                loadComponent: () =>
                    import("./contact/contact.component").then(
                        (c) => c.ContactComponent
                    ),
            },
            {
                path: "agents",
                loadChildren: () =>
                    import("./agents/agents.routes").then((p) => p.routes),
            },
            {
                path: "compare",
                loadComponent: () =>
                    import("./compare/compare.component").then(
                        (c) => c.CompareComponent
                    ),
            },
            {
                path: "faq",
                loadComponent: () =>
                    import("./faq/faq.component").then((c) => c.FaqComponent),
            },
            {
                path: "login",
                loadComponent: () =>
                    import("./login/login.component").then(
                        (c) => c.LoginComponent
                    ),
            },
            {
                path: "register",
                loadComponent: () =>
                    import("./register/register.component").then(
                        (c) => c.RegisterComponent
                    ),
            },
            {
                path: "pricing",
                loadComponent: () =>
                    import("./pricing/pricing.component").then(
                        (c) => c.PricingComponent
                    ),
            },
            {
                path: "properties",
                loadChildren: () =>
                    import("./properties/properties.routes").then(
                        (p) => p.routes
                    ),
            },
            {
                path: "terms-conditions",
                loadComponent: () =>
                    import(
                        "./terms-conditions/terms-conditions.component"
                    ).then((c) => c.TermsConditionsComponent),
            },
            {
                path: "submit-property",
                loadComponent: () =>
                    import("./submit-property/submit-property.component").then(
                        (c) => c.SubmitPropertyComponent
                    ),
            },
            {
                path: "confirmation",
                loadComponent: () =>
                    import("./confirmation/confirmation.component").then(
                        (c) => c.ConfirmationComponent
                    ),
            },
            {
                path: "account",
                loadChildren: () =>
                    import("./account/account.routes").then((p) => p.routes),
            },
        ],
    },
];
