(function (grecaptcha) {
    window.recaptcha = async (siteKey, action) => {
        await grecaptcha.ready(() => { });        

        const token = await grecaptcha.execute(siteKey, { action: action })
        return token;
    }
})(grecaptcha);