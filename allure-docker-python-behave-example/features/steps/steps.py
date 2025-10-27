from behave import given, when, then

@given(u'I\'m on a site')
def step_impl1(context):
    print('first step')

@when(u'I enter {value} on the page')
def step_impl2(context, value):
    print('second step ' + value)

@then(u'I verify is "{status}"')
def step_impl3(context, status):
    print('third step' + status)
    if status == "FAILED":
        assert False, "FAILURE ON PURPOSE"
    if status == "BROKEN":
        raise NotImplementedError('NOT IMPLEMENTED')
