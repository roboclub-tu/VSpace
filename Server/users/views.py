from django.shortcuts import render, redirect
from django.contrib import messages
from django.contrib.auth.decorators import login_required
from django.contrib.auth import logout, login
from .forms import CustomUserCreationForm
from media.models import Media

def register(request):
    if request.method == 'POST':
        form = CustomUserCreationForm(request.POST)
        if form.is_valid():
            user = form.save()
            login(request, user)
            messages.success(request, 'Account created successfully!')
            return redirect('dashboard')
    else:
        form = CustomUserCreationForm()
    return render(request, 'users/register.html', {'form': form})

def home(request):
    if request.user.is_authenticated:
        return redirect('dashboard')
    return render(request, 'home.html')

@login_required
def dashboard(request):
    return render(request, 'users/dashboard.html')

@login_required
def profile_view(request):
    return render(request, 'users/profile.html')

@login_required
def security_view(request):
    if request.method == 'POST':
        current_password = request.POST.get('current_password')
        new_password = request.POST.get('new_password')
        confirm_password = request.POST.get('confirm_password')

        if not request.user.check_password(current_password):
            messages.error(request, 'Current password is incorrect.')
        elif new_password != confirm_password:
            messages.error(request, 'New passwords do not match.')
        elif len(new_password) < 8 or not any(char.isdigit() for char in new_password):
            messages.error(request, 'New password must be at least 8 characters long and contain at least one number.')
        else:
            request.user.set_password(new_password)
            request.user.save()
            messages.success(request, 'Password changed successfully.')
            return redirect('security')

    return render(request, 'users/security.html')

@login_required
def gallery_view(request):
    media_items = Media.objects.all().order_by('-uploaded_at')
    return render(request, 'users/gallery.html', {'media_items': media_items})

def logout_view(request):
    logout(request)
    messages.success(request, 'You have been logged out successfully.')
    return redirect('home')
