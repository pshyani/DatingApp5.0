import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TestErrorsComponent } from './_components/errors/test-errors/test-errors.component';
import { NotFoundComponent } from './_components/errors/not-found/not-found.component';
import { HomeComponent } from './_components/home/home.component';
import { ListsComponent } from './_components/lists/lists.component';
import { MemberDetailComponent } from './_components/members/member-detail/member-detail.component';
import { MemberListComponent } from './_components/members/member-list/member-list.component';
import { MessagesComponent } from './_components/messages/messages.component';
import { AuthGuard } from './_guard/auth.guard';
import { ServerErrorComponent } from './_components/errors/server-error/server-error.component';
import { MemberEditComponent } from './_components/members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guard/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './_components/Admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guard/admin.guard';


const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path: '', 
    runGuardsAndResolvers: 'always', 
    canActivate: [AuthGuard],
    children: [
      {path: 'members', component: MemberListComponent},
      {path: 'members/:username', component: MemberDetailComponent, resolve: {member: MemberDetailedResolver}},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent},
      {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]},
    ]
  }, 
  {path: 'errors', component:TestErrorsComponent},
  {path: 'not-found', component:NotFoundComponent},
  {path: 'server-error', component:ServerErrorComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'},  
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
