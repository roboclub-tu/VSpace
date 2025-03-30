from django import forms
from .models import Media

class MediaUploadForm(forms.ModelForm):
    class Meta:
        model = Media
        fields = ['title', 'media_type', 'file']
        widgets = {
            'description': forms.Textarea(attrs={'rows': 3}),
        }

    def clean_file(self):
        file = self.cleaned_data.get('file')
        media_type = self.cleaned_data.get('media_type')
        
        if media_type == 'image':
            if not file.content_type.startswith('image/'):
                raise forms.ValidationError('File must be an image.')
        elif media_type == 'video':
            if not file.content_type.startswith('video/'):
                raise forms.ValidationError('File must be a video.')
        
        return file 