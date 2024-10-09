import { Routes } from "@angular/router";
import { MyPropertiesComponent } from "./my-properties/my-properties.component";
import { EditPropertyComponent } from "./edit-property/edit-property.component";
import { FavoritesComponent } from "./favorites/favorites.component";
import { ProfileComponent } from "./profile/profile.component"; 
import { AccountComponent } from "./account.component";
import { MyPymentsComponent } from "./my-payments/my-payments.component";


export const routes: Routes = [
    {
        path: '',
        component: AccountComponent, children: [
            { path: '', redirectTo: 'profile', pathMatch: 'full' },
            { path: 'my-properties', component: MyPropertiesComponent },
            { path: 'my-properties/:id', component: MyPymentsComponent },
            { path: 'favorites', component: FavoritesComponent },
            { path: 'profile', component: ProfileComponent }
        ]
    }
];