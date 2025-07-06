using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E_CommerceDataAccess.DTO;
using FluentValidation;

namespace E_CommerceDataBusiness.Validator
{
    public class CartDTOValidator : AbstractValidator<CartDTO>
    {
        public CartDTOValidator()
        {
            RuleFor(x => x.UserID).NotEmpty();
            RuleFor(x => x.Items).NotEmpty();
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        }
    }

    public class CategoryDTOValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class CategoryCreateDTOValidator : AbstractValidator<CategoryCreateDTO>
    {
        public CategoryCreateDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class CategoryUpdateDTOValidator : AbstractValidator<CategoryUpdateDTO>
    {
        public CategoryUpdateDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class OrderDTOValidator : AbstractValidator<OrderDTO>
    {
        public OrderDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
            RuleForEach(x => x.OrderItems).SetValidator(new OrderItemDTOValidator());
        }
    }

    public class OrderCreateDTOValidator : AbstractValidator<OrderCreateDTO>
    {
        public OrderCreateDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OrderItems).NotEmpty();
            RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDTOValidator());
        }
    }

    public class OrderUpdateDTOValidator : AbstractValidator<OrderUpdateDTO>
    {
        public OrderUpdateDTOValidator()
        {
            RuleFor(x => x.Status).NotEmpty();
            RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDTOValidator());
        }
    }

    public class ProductCreateDTOValidator : AbstractValidator<ProductCreateDTO>
    {
        public ProductCreateDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ImageFile).NotNull();
        }
    }

    public class ProductUpdateDTOValidator : AbstractValidator<ProductUpdateDTO>
    {
        public ProductUpdateDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        }
    }

    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
        }
    }

    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
    {
        public ForgetPasswordDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword);
        }
    }

    public class VerfiyCodeDtoValidator : AbstractValidator<VerfiyCodeDto>
    {
        public VerfiyCodeDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.CodeOTP).NotEmpty().Length(6);
        }
    }

    
    public class OrderItemDTOValidator : AbstractValidator<OrderItemDTO>
    {
        public OrderItemDTOValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    public class OrderItemCreateDTOValidator : AbstractValidator<OrderItemCreateDTO>
    {
        public OrderItemCreateDTOValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

}
