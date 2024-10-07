import { Routes } from "@angular/router";
import { PropertiesComponent } from "./properties.component";
import { PropertyComponent } from "./property/property.component";

export const routes: Routes = [
    { path: '', component: PropertiesComponent, pathMatch: 'full' },
    { path: ':id', component: PropertyComponent }
];