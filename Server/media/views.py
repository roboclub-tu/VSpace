from django.shortcuts import render, redirect
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from .forms import MediaUploadForm
from .models import Media

# Create your views here.

@login_required
def upload_media(request):
    if request.user.role.lower() != 'participant':
        messages.error(request, f'Only Participants can upload media. Your role is: {request.user.role}')
        return redirect('dashboard')

    if request.method == 'POST':
        form = MediaUploadForm(request.POST, request.FILES)
        if form.is_valid():
            media = form.save(commit=False)
            media.user = request.user
            
            # Check if user has enough tokens
            if request.user.tokens_amount < media.token_cost:
                messages.error(request, f'Not enough tokens. Required: {media.token_cost}, Available: {request.user.tokens_amount}')
                return redirect('dashboard')
            
            # Deduct tokens
            request.user.tokens_amount -= media.token_cost
            request.user.save()
            
            media.save()
            messages.success(request, f'Media uploaded successfully! {media.token_cost} token(s) deducted.')
            return redirect('dashboard')
    else:
        form = MediaUploadForm()
    
    return render(request, 'media/upload.html', {'form': form})
