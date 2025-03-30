from django import forms
from django.contrib.auth.forms import UserCreationForm
from .models import CustomUser

class CustomUserCreationForm(UserCreationForm):
    email = forms.EmailField(required=True)
    
    class Meta:
        model = CustomUser
        fields = ('username', 'email', 'role', 'password1', 'password2')
        
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        # Add Bootstrap classes
        for field in self.fields.values():
            field.widget.attrs['class'] = 'form-control'
        # Remove password validation messages
        self.fields['password1'].help_text = ''
        self.fields['password2'].help_text = ''
        # Remove password validation
        self.fields['password1'].validators = []
        self.fields['password2'].validators = [] 