import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './_components/nav/nav.component';
import { HomeComponent } from './_components/home/home.component';
import { RegisterComponent } from './_components/register/register.component';
import { MemberListComponent } from './_components/members/member-list/member-list.component';
import { MemberDetailComponent } from './_components/members/member-detail/member-detail.component';
import { ListsComponent } from './_components/lists/lists.component';
import { MessagesComponent } from './_components/messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorsComponent } from './_components/errors/test-errors/test-errors.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './_components/errors/not-found/not-found.component';
import { ServerErrorComponent } from './_components/errors/server-error/server-error.component';
import { MemberCardComponent } from './_components/members/member-card/member-card.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { MemberEditComponent } from './_components/members/member-edit/member-edit.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { PhotoEditorComponent } from './_components/members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { MemberMessagesComponent } from './_components/members/member-messages/member-messages.component';
import { AdminPanelComponent } from './_components/Admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './_components/Admin/user-management/user-management.component';
import { PhotoManagementComponent } from './_components/Admin/photo-management/photo-management.component';
import { RolesModelComponent } from './modals/roles-model/roles-model.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MemberDetailComponent,
    ListsComponent,
    MessagesComponent,
    TestErrorsComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberCardComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    TextInputComponent,
    DateInputComponent,
    MemberMessagesComponent,
    AdminPanelComponent,
    HasRoleDirective,
    UserManagementComponent,
    PhotoManagementComponent,
    RolesModelComponent,
    ConfirmDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,    
    ReactiveFormsModule,
    SharedModule,    
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass:ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass:JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass:LoadingInterceptor, multi: true}, 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
