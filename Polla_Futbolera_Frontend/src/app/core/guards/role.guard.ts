import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Role } from '../../infrastructure/api/auth/auth-api.types';
import { AuthStore } from '../../modules/auth/stores/auth.store';

export function roleGuard(allowedRoles: Role[]): CanActivateFn {
  return () => {
    const authStore = inject(AuthStore);
    const router = inject(Router);

    if (!authStore.isAuthenticated()) return router.createUrlTree(['/auth/login']);

    const role = authStore.role();
    if (role && allowedRoles.includes(role)) return true;

    return router.createUrlTree(['/']);
  };
}
