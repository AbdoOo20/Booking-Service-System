import { Routes } from "@angular/router";
import { AgentComponent } from "./agent/agent.component";
import { AgentsComponent } from "./agents.component";

export const routes: Routes = [
    { path: '', component: AgentsComponent, pathMatch: 'full' },
    { path: ':id', component: AgentComponent }
];