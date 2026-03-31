import {
  Component, OnInit, AfterViewInit, ElementRef, ViewChild, NgZone
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, AfterViewInit {
  @ViewChild('googleBtn') googleBtnRef!: ElementRef;

  form!: FormGroup;
  loading = false;
  googleLoading = false;
  errorMsg = '';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private zone: NgZone
  ) {}

  ngOnInit(): void {
    // Redirect if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
      return;
    }

    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngAfterViewInit(): void {
    this.initGoogleSignIn();
  }

  // ── Google Sign-In setup ─────────────────────────────
  private initGoogleSignIn(): void {
    const google = (window as any).google;
    if (!google?.accounts?.id) {
      // Retry after 500ms if SDK not loaded yet
      setTimeout(() => this.initGoogleSignIn(), 500);
      return;
    }

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.handleGoogleResponse(response),
      auto_select: false,
      cancel_on_tap_outside: true
    });

    google.accounts.id.renderButton(this.googleBtnRef.nativeElement, {
      theme: 'outline',
      size: 'large',
      width: 340,
      text: 'signin_with',
      shape: 'rectangular',
      logo_alignment: 'center'
    });
  }

  // ── Google callback ──────────────────────────────────
  private handleGoogleResponse(response: any): void {
    this.zone.run(() => {
      this.googleLoading = true;
      this.errorMsg = '';

      this.authService.googleLogin(response.credential).subscribe({
        next: () => {
          this.googleLoading = false;
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.googleLoading = false;
          this.errorMsg = err.error?.message || 'Google sign-in failed. Please try again.';
        }
      });
    });
  }

  // ── Local Login submit ───────────────────────────────
  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMsg = '';

    this.authService.login(this.form.value).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error?.message || 'Login failed. Please try again.';
      }
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  get email() { return this.form.get('email')!; }
  get password() { return this.form.get('password')!; }
}
